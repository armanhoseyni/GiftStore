using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class SmsBank
    {
        [Key]
        public int Id { get; set; }
        public string phone { get; set; }
        public string Code{ get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
