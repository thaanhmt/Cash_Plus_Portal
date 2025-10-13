using Microsoft.EntityFrameworkCore;

namespace IOITWebApp31.Models.Localization
{
    public class LocalizationDbContext : DbContext
    {
        public DbSet<Culture> Cultures { get; set; }
        public DbSet<Resource> Resources { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseInMemoryDatabase();
            optionsBuilder.UseInMemoryDatabase("___Shared_Database___");
        }
    }
}
