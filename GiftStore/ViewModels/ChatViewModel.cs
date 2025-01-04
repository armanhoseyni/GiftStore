using GiftStore.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftStore.ViewModels
{
    public class ChatViewModel
    {
        public int Id { get; set; }

        [ForeignKey("Tickets")]
        public int TicketId { get; set; }

       
        public string message { get; set; }

     

        public DateTime SendDate { get; set; }

        public int Sender { get; set; }

    }
}
