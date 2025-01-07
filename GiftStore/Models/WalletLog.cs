using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class WalletLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }

        public string Type { get; set; } // "Income" or "Out"
        public decimal Amount { get; set; } // Amount of the transaction
        public DateTime LogDate { get; set; } // Date of the transaction
        public bool Status { get; set; } // "Waiting" or "Success"

        public Users user { get; set; } // N

    }
}
