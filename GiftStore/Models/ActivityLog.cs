using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class ActivityLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }

        public string Description { get; set; } // "Income" or "Out"
        public string UserRole { get; set; } // "Income" or "Out"
        public string Name { get; set; } // "Income" or "Out"
       
        public DateTime LogDate { get; set; } // Date of the transaction
     
        public Users user { get; set; } // N
    }
}
