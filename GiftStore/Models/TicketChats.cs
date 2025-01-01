using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class TicketChats
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tickets")]
        public int TicketId { get; set; }

        public Tickets tickets { get; set; }

        public string message { get; set; }
       
        public string? DocumentPath { get; set; }

       
        public DateTime SendDate { get; set; }

        public int Sender { get; set; }
     
    }
}
