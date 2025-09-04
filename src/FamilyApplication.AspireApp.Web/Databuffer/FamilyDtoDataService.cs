using System.Threading.RateLimiting;
using BlazorServerCommon.Extensions;
using Eiriklb.Utils;
using FamilyApplication.AspireApp.Web.CosmosDb;
using FamilyApplication.AspireApp.Web.CosmosDb.Family;
using FamilyApplication.AspireApp.Web.CosmosDb.Notification;
using FamilyApplication.AspireApp.Web.CosmosDb.User;
using FamilyApplication.AspireApp.Web.Notifications;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FamilyApplication.AspireApp.Web.Databuffer
{
    public class FamilyDtoDataService(GlobalVm vm, AppDbContext dbContext, UserDtoDataService userDtoDataService, NotificationManager notificationManager)
    {
        public async Task Initialize(CancellationToken token)
        {


            var families = await dbContext.FamilyDtos.ToListAsync(token);

            foreach (var family in families)
                vm.FamilyDtos.Add(family);

        }


        public async Task AddUpdateFamilyEvent(FamilyEvent fe, FamilyDto familyDto, UserDto performingUser, CancellationToken token)
        {
            var trackedEntity = dbContext.ChangeTracker.Entries<FamilyDto>()
                .FirstOrDefault(a => a.Entity.Id == familyDto.Id);

            if (trackedEntity == null)
                dbContext.FamilyDtos.Attach(familyDto);

            var familyEventExits = familyDto.FamilieEvents.FirstOrDefault(a => a.Id == fe.Id);

            var isNew = false;

            if (familyEventExits != null)
            {
                Eiriklb.Utils.ObjectSync.Instance.Update(familyEventExits, fe);
            }
            else
            {
                isNew = true;
                familyDto.FamilieEvents.SortedInsert(fe, a => a.Date, a => a.Time);
            }

            var familyUsers = vm.UserDtos.Where(a => a.FamilyId == familyDto.Id);

            string notificationText;

            if (isNew)
                notificationText = $"{performingUser.FirstName} har opprettet event {fe.Title} som går den {fe.Date.ToShortDateString()}";
            else
                notificationText = $"{performingUser.FirstName} har endret event {fe.Title} som går den {fe.Date.ToShortDateString()}";

            var listNotification = (from user in familyUsers
                                   select new NotificationDto()
                                   {
                                       CreatedAt = DateTime.Now,
                                       FamilyId = familyDto.Id,
                                       Id = Guid.NewGuid().ToString(),
                                       NotificationDtoType = NotificationDtoType.FamilyEvent,
                                       Text = notificationText,
                                       Title = (isNew) ? "Ny event opprettet" : "Endret event",
                                       ReferenceId = fe.Id.ToString(),
                                       UserId = user.Id,
                                       IsUnread = true,
                                        CreatedById = performingUser.Id
                                   }).ToList();

            userDtoDataService.AddNotificationsToUsers(listNotification);
            await dbContext.SaveChangesAsync(token);
            await notificationManager.WebPushNotify(listNotification, userDtoDataService, token);

        }

        public async Task DeleteFamilyEvent(FamilyEvent fe, FamilyDto familyDto, UserDto performingUser, CancellationToken token)
        {
            var trackedEntity = dbContext.ChangeTracker.Entries<FamilyDto>()
                .FirstOrDefault(a => a.Entity.Id == familyDto.Id);
            if (trackedEntity == null)
                dbContext.FamilyDtos.Attach(familyDto);
            var familyEventExits = familyDto.FamilieEvents.FirstOrDefault(a => a.Id == fe.Id);

            if (familyEventExits == null)
                return;

            familyDto.FamilieEvents.Remove(fe);

            var familyUsers = vm.UserDtos.Where(a => a.FamilyId == familyDto.Id);

            string notificationText;

                notificationText = $"{performingUser.FirstName} har avbrutt event {fe.Title} som går den {fe.Date.ToShortDateString()}";
            var listNotification = (from user in familyUsers
                                    select new NotificationDto()
                                    {
                                        CreatedAt = DateTime.Now,
                                        FamilyId = familyDto.Id,
                                        Id = Guid.NewGuid().ToString(),
                                        NotificationDtoType = NotificationDtoType.FamilyEvent,
                                        Text = notificationText,
                                        ReferenceId = fe.Id.ToString(),
                                        UserId = user.Id,
                                        IsUnread = true,
                                        Title = "Slettet event",
                                         CreatedById = performingUser.Id
                                    }).ToList();

            userDtoDataService.AddNotificationsToUsers(listNotification);
            await dbContext.SaveChangesAsync(token);
            await notificationManager.WebPushNotify(listNotification, userDtoDataService, token);
        }






    }
}
