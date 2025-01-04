using GiftStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;

namespace GiftStore.Data
{
    public class Db_API : DbContext
    {
        public DbSet<Users> users { get; set; }
        public DbSet<GiftCards> giftCards { get; set; }
        public DbSet<Tickets> tickets{ get; set; }
        public DbSet<Factors> factors{ get; set; }
        public DbSet<TelegramStars> telegramStars{ get; set; }
        public DbSet<Contact_us> contact_Us{ get; set; }
        public DbSet<UserStarsLog> userStarsLogs { get; set; }
        public DbSet<TicketChats> ticketChats{ get; set; }
        public DbSet<WalletLog> walletLogs{ get; set; }
        public DbSet<ActivityLog> activityLogs{ get; set; }
      

        public Db_API(DbContextOptions<Db_API> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Factors>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Factors>()
                .HasOne(f => f.GiftCard)
                .WithMany()
                .HasForeignKey(f => f.GiftId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete
            modelBuilder.Entity<UserStarsLog>()
             .HasOne(f => f.user)
             .WithMany()
             .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<WalletLog>()
              .HasOne(w => w.user)
              .WithMany(u => u.WalletLogs)
              .HasForeignKey(w => w.UserId);

            modelBuilder.Entity<ActivityLog>()
            .HasOne(w => w.user)
            .WithMany(u => u.activityLogs)
            .HasForeignKey(w => w.UserId);



            modelBuilder.Entity<TicketChats>()
       .HasOne(f => f.tickets)
       .WithMany()
       .HasForeignKey(f => f.TicketId);

            modelBuilder.Entity<Tickets>()
  .HasOne(f => f.user)
  .WithMany()
  .HasForeignKey(f => f.UserId);

        }

    }
}
