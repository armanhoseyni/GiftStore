using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class TelegramStars
    {
        [Key]
        public int Id { get; set; }

        public int StarsPerADollar{ get; set; }
        public int MinStars{ get; set; }
    }
}
