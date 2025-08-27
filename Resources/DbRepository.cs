using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entity;

namespace minimal_api.Resources
{
    public class DbRepository : DbContext
    {

        private readonly IConfiguration _configuration;
        public DbRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DbSet<AdminEntity> Admins { get; set; } = default!;

        public DbSet<VehicleEntity> Vehicles { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminEntity>().HasData(
                new AdminEntity
                {
                    Id = 1,
                    Email = "Alexandre@admin.com",
                    Password = "admin123",
                    Perfil = "Admin"
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {


                var connectionString = _configuration.GetConnectionString("mysql")?.ToString();
                if (!string.IsNullOrEmpty(connectionString))
                    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                else
                    optionsBuilder.UseMySql("Server=localhost;Database=minimal_api;User=root;Password=root;", ServerVersion.AutoDetect("Server=localhost;Database=minimal_api;User=root;Password=root;"));

            }
        }
    }
}
