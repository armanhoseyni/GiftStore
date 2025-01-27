﻿using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        public string ?FirstName { get; set; }
        public string? Username{ get; set; }

        public string? LastName { get; set; }
        public string? Email{ get; set; }

        public string Phone { get; set; }
        public string? Password { get; set; }
        public string? RePassword { get; set; }
        public int Stars { get; set; }
        
        public bool Active { get; set; }

        public DateTime  RegisterDate{ get; set; }
        public string?  Role{ get; set; }

        public double? wallet {  get; set; }
        public ICollection<WalletLog> WalletLogs { get; set; } // Navigation property to WalletLog
        public ICollection<ActivityLog> activityLogs { get; set; } // Navigation property to WalletLog

    }
}
