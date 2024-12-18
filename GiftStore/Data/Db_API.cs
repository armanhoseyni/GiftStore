using GiftStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace GiftStore.Data
{
    public class Db_API : DbContext
    {
        public DbSet<Users> users { get; set; }
        public DbSet<GiftCards> giftCards { get; set; }
        public DbSet<Tickets> tickets{ get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=DB_GiftStore;Integrated Security=True;Trust Server Certificate=True");
        }
    }
}
