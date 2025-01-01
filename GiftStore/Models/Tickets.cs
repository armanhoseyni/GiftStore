using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class Tickets
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }
        public Users user { get; set; }

        public int Importance { get; set; } // Assuming importance is an integer value
        public string Description { get; set; }
        public string Title { get; set; }
       public string DocumentPath { get; set; }
        
        public string Status { get; set; }//تیکت‌های باز، پاسخ‌داده‌شده و بسته‌شده.

        public DateTime SendDate { get; set; } 


    }
}
