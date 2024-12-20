using System.ComponentModel.DataAnnotations.Schema;

namespace GiftStore.ViewModels
{
    public class AddTicketViewModel
    {

        public int Importance { get; set; } // Assuming importance is an integer value
        public string Description { get; set; }
        public string Title { get; set; }
        //   public IFormFile? Document { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }

    }
}
