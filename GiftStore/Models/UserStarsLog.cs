using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftStore.Models
{
    public class UserStarsLog
    {
        [Key] public int Id { get; set; }
        [ForeignKey("Users")]

        public int UserId { get; set; }
        public int Star { get; set; }
        public string Type { get; set; } //income or out
        public string? Username { get; set; } 

        public DateTime LogDate{get; set;}

        public string Status { get; set; } //Waiting or successed
        public Users user{ get; set; }


    }
}
