using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entity;

namespace minimal_api.Resources
{
    public class DbRepository : DbContext
    {

        public DbSet<AdminEntity> Admins { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           optionsBuilder.UseMySql("string de conexao",ServerVersion.AutoDetect("string de conexao"));


        }
    }
}
