using Console_MicrosoftGraphEmail.Models.ConnectWise;
using Microsoft.EntityFrameworkCore;


namespace Console_MicrosoftGraphEmail.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomTicket>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("Tickets");
            });
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<CustomTicket> Tickets { get; set; }

    }
}
