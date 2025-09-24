using Eiriklb.Utils;
using FamilyApplication.AspireApp.Web.CosmosDb;
using FamilyApplication.AspireApp.Web.CosmosDb.BlackBoard;
using FamilyApplication.AspireApp.Web.CosmosDb.Family;
using FamilyApplication.AspireApp.Web.CosmosDb.Notification;
using FamilyApplication.AspireApp.Web.CosmosDb.User;
using FamilyApplication.AspireApp.Web.Notifications;
using FamilyApplication.AspireApp.Web.Sessions;
using Microsoft.Azure.Cosmos.Core.Networking;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Reflection.Metadata;

namespace FamilyApplication.AspireApp.Web.Databuffer
{
    public class UserDtoDataService(GlobalVm vm, AppDbContext dbContext, NotificationManager notificationManager)
    {
        public async Task Initialize(CancellationToken token)
        {
            var users = await dbContext.UserDtos.ToListAsync(token);

            foreach (var user in users)
                vm.UserDtos.Add(user);

        }

        public async Task Save(UserDto userDto, CancellationToken token)
        {
            // Check if the entity is already being tracked
            var trackedEntity = dbContext.ChangeTracker.Entries<UserDto>()
                .FirstOrDefault(e => e.Entity.Id == userDto.Id);

            if (trackedEntity != null)
            {
                // If already tracked, update its properties directly
                trackedEntity.CurrentValues.SetValues(userDto);
            }
            else
            {
                // Attach and mark as modified if not tracked
                dbContext.UserDtos.Attach(userDto);
            }

            await dbContext.SaveChangesAsync(token);

            // Update the VM
            var existing = vm.UserDtos.Single(a => a.Id == userDto.Id);
            ObjectSync.Instance.Update(existing, userDto);
        }

        public async Task DeleteSaveGoal(string userId, string saveGoalId, CancellationToken token)
        {


            var userDto = vm.UserDtos.Single(a => a.Id == userId);
            ArgumentNullException.ThrowIfNull(userDto.Wallet.SaveGoals);

            var saveGoal = userDto.Wallet.SaveGoals.Single(a => a.Id == saveGoalId);
            userDto.Wallet.SaveGoals.Remove(saveGoal);
            await Save(userDto, token);
        }

        public async Task AddUpdateSaveGoal(string userId, UserWalletSaveGoal saveGoal, CancellationToken token)
        {


            var user = vm.UserDtos.Single(a => a.Id == userId);
            user.Wallet.SaveGoals ??= new();

            var existing = user.Wallet.SaveGoals.SingleOrDefault(a => a.Id == saveGoal.Id);
            if (existing == null)
                user.Wallet.SaveGoals.Add(saveGoal);
            else
                ObjectSync.Instance.Update(existing, saveGoal);

            await Save(user, token);
        }

        public async Task PerformTodoFromBlackboard(BlackBoardTodoDto dto, string belongsToUserId, string performedById, CancellationToken token)
        {

            dbContext.BlackBoardDtos.Attach(dto);
            dto.ListPerformed ??= new();

            dto.ListPerformed.Insert(0, new BlackBoardTodoDtoPerformed()
            {
                At = DateTime.UtcNow,
                UserId = performedById,
                Id = Guid.NewGuid().ToString()
            });


            var userBelongsTo = vm.UserDtos.Single(a => a.Id == belongsToUserId);
            var userPerformedBy = vm.UserDtos.Single(a => a.Id == performedById);
            ArgumentNullException.ThrowIfNull(userBelongsTo.FamilyId);

            var clonedFamilyTodo = dto.Todo.Clone();

            clonedFamilyTodo.UserIdBelongsTo = belongsToUserId;
            clonedFamilyTodo.Id = Guid.NewGuid().ToString();
            clonedFamilyTodo.IsDone = true;

            List<NotificationDto> listNotifications = new();

            if (belongsToUserId != performedById)
            {
                var notification = new NotificationDto()
                {
                    FamilyId = userBelongsTo.FamilyId!,
                    Id = Guid.NewGuid().ToString(),
                    Text = $"{userPerformedBy.Surname} har tildelt oppgave {clonedFamilyTodo.Tittel} til deg",
                    NotificationDtoType = NotificationDtoType.Todo,
                    ReferenceId = dto.Todo.Id,
                    UserId = userBelongsTo.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = performedById,
                    Title = "Tildelt oppgave"
                };
            }
            var parents = vm.UserDtos.Where(a => a.FamilyId == userBelongsTo.FamilyId && a.UserType == UserType.Parent);

            listNotifications.AddRange(parents
                .Select(parent => new NotificationDto()
                {
                    FamilyId = userBelongsTo.FamilyId,
                    Id = Guid.NewGuid().ToString(),
                    NotificationDtoType = NotificationDtoType.Todo,
                    ReferenceId = clonedFamilyTodo.Id,
                    Text = $"{userPerformedBy.Surname} har sendt oppgave {clonedFamilyTodo.Tittel} til godkjenning",
                    UserId = parent.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = performedById,
                    Title = "Oppgave til godkjenning"
                }));

            this.AddNotificationsToUsers(listNotifications);


            token.ThrowIfCancellationRequested();
            userBelongsTo.TodosToApprove.Add(clonedFamilyTodo);
            var changed = await dbContext.SaveChangesAsync(token);
            await notificationManager.WebPushNotify(listNotifications, this, token);
        }

        public async Task DisableEnableNotification(UserDto? user, string lastChangeById)
        {
            try
            {

                if (user == null)
                    return;

                using var task = vm.AddTask("Subscriber");
                var token = task.GetToken();

                var clone = new UserDto();
                Eiriklb.Utils.ObjectSync.Instance.Update(clone, user);
                clone.DisableNotifications = !clone.DisableNotifications;
                if (clone.DisableNotifications == false)
                    clone.NotificationSubscription = null;

                clone.LastChangeBy = lastChangeById;
                clone.LastChangedAt = DateTime.UtcNow;
                await Save(clone, token);

            }
            catch (Exception ex)
            {
                vm.AddException(ex);
            }
        }

        public async Task UpdateTodoToApprove(FamilyTodoDto clonedDto, string belongsToUserId, string performedById, CancellationToken token)
        {
            var userBelongsTo = vm.UserDtos.Single(a => a.Id == belongsToUserId);
            var userPerformedBy = vm.UserDtos.Single(a => a.Id == performedById);
            ArgumentNullException.ThrowIfNull(userBelongsTo.FamilyId);


            var orgTodo = userBelongsTo.TodosToApprove.Single(a => a.Id == clonedDto.Id);

            var changes = ObjectSync.Instance.Update(orgTodo, clonedDto);

            if (changes.Count > 0)
            {
                await dbContext.SaveChangesAsync(token);
            }

        }

        public async Task ApproveTodoToApprove(FamilyTodoDto todo, string belongsToUserId, string userIdPerformedby, CancellationToken token)
        {
            var belongsToUser = vm.UserDtos.Single(a => a.Id == belongsToUserId);
            var userPerformedBy = vm.UserDtos.Single(a => a.Id == userIdPerformedby);
            ArgumentNullException.ThrowIfNull(belongsToUser.FamilyId);

            var todoInstance = belongsToUser.TodosToApprove.Single(a => a.Id == todo.Id);


            var parents = vm.UserDtos.Where(a => a.FamilyId == belongsToUser.FamilyId && a.UserType == CosmosDb.User.UserType.Parent);

            var payAmount = todo.PayAmount != null ? todo.PayAmount.Value : 0;
            var newAmount = payAmount + belongsToUser.Wallet.Amount;

            belongsToUser.Wallet.Amount = newAmount;
            belongsToUser.Wallet.LastChangeBy = userIdPerformedby;
            belongsToUser.Wallet.LastChangedAt = DateTime.UtcNow;
            belongsToUser.TodosToApprove.Remove(todoInstance);
            if (belongsToUser.Wallet.Transactions == null)
            {
                belongsToUser.Wallet.Transactions = new();
            }
            ;

            belongsToUser.Wallet.Transactions.Insert(0, new UserWalletTransactions()
            {
                Amount = todo.PayAmount ?? 0,
                ChangeBy = userIdPerformedby,
                ChangedAt = DateTimeOffset.UtcNow,
                Reason = "Oppgave utført: " + todo.Tittel,
                Id = Guid.NewGuid().ToString()
            });

            List<NotificationDto> listNotification = new();

            if (payAmount > 0)
            {
                var walletIncreaseNotification = new NotificationDto()
                {
                    FamilyId = belongsToUser.FamilyId,
                    Id = Guid.NewGuid().ToString(),
                    NotificationDtoType = NotificationDtoType.Wallet,
                    ReferenceId = null,
                    Text = $"{userPerformedBy.Surname} har satt inn kr {payAmount} i din lommebok",
                    UserId = belongsToUser.Id,
                    IsUnread = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = userIdPerformedby,
                    Title = "Satt in penger"
                };
                listNotification.Add(walletIncreaseNotification);
            }
            else
            {

                var todoCompleted = new NotificationDto()
                {
                    FamilyId = belongsToUser.FamilyId,
                    Id = Guid.NewGuid().ToString(),
                    NotificationDtoType = NotificationDtoType.Todo,
                    IsUnread = true,
                    Text = $"{userPerformedBy.Surname} har godkjent og sluttstilt oppgaven {todo.Tittel}",
                    UserId = belongsToUser.Id,
                    ReferenceId = null, // Because it is deleted now,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = userIdPerformedby,
                    Title = "Godkjent oppgave"
                };
                listNotification.Add(todoCompleted);
            }
            var todoCompletedParents = from parent in parents
                                       select new NotificationDto()
                                       {
                                           FamilyId = belongsToUser.FamilyId,
                                           Id = Guid.NewGuid().ToString(),
                                           NotificationDtoType = NotificationDtoType.Todo,
                                           IsUnread = true,
                                           Text = $"{userPerformedBy.Surname} har godkjent og sluttstilt oppgaven {todo.Tittel}",
                                           UserId = parent.Id,
                                           ReferenceId = null, // Because it is deleted now,
                                           CreatedAt = DateTime.UtcNow,
                                           CreatedById = userIdPerformedby,
                                           Title = "Godkjent oppgave"
                                       };
            listNotification.AddRange(todoCompletedParents);

            AddNotificationsToUsers(listNotification);
            await dbContext.SaveChangesAsync(token);
            await notificationManager.WebPushNotify(listNotification,this, token);

        }

        public void AddNotificationsToUsers(List<NotificationDto> listNotification)
        {
            var joinUsers = from user in vm.UserDtos
                            join notification in listNotification on user.Id equals notification.UserId
                            into joinedNotifications
                            select new
                            {
                                user,
                                joinedNotifications
                            };

            foreach (var j in joinUsers)
            {
                var trackedEntity = dbContext.ChangeTracker.Entries<UserDto>()
              .FirstOrDefault(a => a.Entity.Id == j.user.Id);

                if (trackedEntity == null)
                    dbContext.UserDtos.Attach(j.user);

                foreach (var notification in j.joinedNotifications)
                    j.user.Notifications.Add(notification);
            }

        }


        public async Task DeleteTodoToApprove(string todoId, string belongsToUserId, CancellationToken token)
        {

            var userBelongsTo = vm.UserDtos.Single(a => a.Id == belongsToUserId);
            var todo = userBelongsTo.TodosToApprove.Single(a => a.Id == todoId);
            userBelongsTo.TodosToApprove.Remove(todo);
            await dbContext.SaveChangesAsync(token);
        }
    }
}
