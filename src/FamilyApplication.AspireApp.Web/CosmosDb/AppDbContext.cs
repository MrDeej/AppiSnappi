using FamilyApplication.AspireApp.Web.CosmosDb.BlackBoard;
using FamilyApplication.AspireApp.Web.CosmosDb.Family;
using FamilyApplication.AspireApp.Web.CosmosDb.User;
using Microsoft.EntityFrameworkCore;

namespace FamilyApplication.AspireApp.Web.CosmosDb
{
    public class AppDbContext : DbContext

    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<FamilyDto> FamilyDtos { get; set; }
        public DbSet<UserDto> UserDtos { get; set; }

        public DbSet<BlackBoardTodoDto> BlackBoardDtos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FamilyDto>(entity =>
                {
                    entity.ToContainer("FamilyDto")
                          .HasPartitionKey(f => f.FamilyId)
                          .HasKey(f => f.Id);


                    entity.HasDiscriminator<string>("__type")
                          .HasValue<FamilyDto>("FamilyDto");

                    entity.OwnsMany(a => a.FamilieEvents);
                });
            modelBuilder.Entity<UserDto>(entity =>
                {
                    entity.ToContainer("UserDto")
                          .HasPartitionKey(f => f.FamilyId)
                          .HasKey(f => f.Id);

                    entity.HasDiscriminator<string>("__type")
                          .HasValue<UserDto>("UserDto");

                    entity.OwnsOne(u => u.Wallet, wallet =>
                    {
                        wallet.OwnsMany(w => w.SaveGoals);
                        wallet.OwnsMany(w => w.Incoming);
                        wallet.OwnsMany(w => w.Transactions);
                    });

                    entity.OwnsOne(u => u.NotificationSubscription);

                    entity.OwnsMany(u => u.TodosToApprove);
                    entity.OwnsMany(n => n.Notifications);
                });



            modelBuilder.Entity<BlackBoardTodoDto>(entity =>
            {
                entity.ToContainer("BlackBoardTodoDto")
                .HasPartitionKey(f => f.FamilyId)
                .HasKey(f => f.Id);

                entity.OwnsOne(a => a.Todo);

                entity.OwnsMany(a => a.ListPerformed);

                entity.HasDiscriminator<string>("__type")
                      .HasValue<BlackBoardTodoDto>("BlackBoardTodoDto");
            });


        }
    }
}
