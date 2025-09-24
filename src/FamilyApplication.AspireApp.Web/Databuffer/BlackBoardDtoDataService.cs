using BlazorServerCommon.Extensions;
using FamilyApplication.AspireApp.Web.CosmosDb;
using FamilyApplication.AspireApp.Web.CosmosDb.BlackBoard;
using Microsoft.EntityFrameworkCore;

namespace FamilyApplication.AspireApp.Web.Databuffer
{
    public class BlackBoardDtoDataService(GlobalVm vm, AppDbContext dbContext)
    {
        public async Task Initialize(CancellationToken token)
        {
            var blackBoardDtos = await dbContext.BlackBoardDtos.ToListAsync(token);

            foreach (var blackBoardDto in blackBoardDtos
                            .OrderByDescending(dto => (dto.CreatedAt ?? new DateTime()) >= DateTime.UtcNow.AddDays(-7)) // Recent items first
                            .ThenByDescending(dto => (dto.ListPerformed?.Count ?? 0) > 0) // Items with a performed count
                            .ThenBy(dto => dto.ListPerformed?.Count ?? 0))
                vm.BlackBoardDtos.Add(blackBoardDto);

        }



        public async Task AddBlackBoardTodo(BlackBoardTodoDto blackBoardDto, CancellationToken token)
        {
            dbContext.BlackBoardDtos.Add(blackBoardDto);
            await dbContext.SaveChangesAsync(token);
            vm.BlackBoardDtos.Insert(0, blackBoardDto);
        }

        public async Task EditBlackBoardTodo(BlackBoardTodoDto dto, CancellationToken token)
        {
            var existing = vm.BlackBoardDtos.Single(a => a.Id == dto.Id);
            dbContext.BlackBoardDtos.Attach(existing);
            ObjectSync.Instance.Update(existing, dto);
            await dbContext.SaveChangesAsync(token);
        }

        public async Task DeleteBlackBoardTodo(string id, CancellationToken token)
        {
            var existing = vm.BlackBoardDtos.Single(a => a.Id == id);
            dbContext.BlackBoardDtos.Remove(existing);
            await dbContext.SaveChangesAsync(token);
            vm.BlackBoardDtos.Remove(existing);
        }
    }
}
