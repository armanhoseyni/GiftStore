using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class Notifications
    {
        [Key]
        public int Id { get; set; }
        public int Status { get; set; }

        public string Title { get; set; }
        public string ?Description{ get; set; }

        public string? Type { get; set; }
        public string? Username { get; set; }
        public string? showTo { get; set; }
        public string? DocumentPath { get; set; }

        
        
        public DateTime DateOfNotification{ get; set; }
 
    }
}
