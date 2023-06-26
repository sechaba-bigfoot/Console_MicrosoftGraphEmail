using Console_MicrosoftGraphEmail.Models.ConfigurationModels;
using Console_MicrosoftGraphEmail.Models.ConnectWise;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Console_MicrosoftGraphEmail.Data
{
    public class ApplicationDbContext : DbContext
    {
        string customTicketsViewOrTableName;
        public ApplicationDbContext(DbContextOptions options, IOptions<ApplicationConfigurations> applicationOpions) : base(options)
        {
            customTicketsViewOrTableName = applicationOpions.Value.TableOrViewName;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomTicket>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(customTicketsViewOrTableName);
            });
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<CustomTicket> Tickets { get; set; }

    }
}
