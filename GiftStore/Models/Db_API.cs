using GiftStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace API_M.Models
{
    public class Db_API: DbContext
    {
        public DbSet<Users> users{ get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=DB_GiftStore;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
        }
    }
}
