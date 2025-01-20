using System.ComponentModel.DataAnnotations;

namespace GiftStore.ViewModels
{
    public class ViewModelResponseToTickets
    {
        [MaxLength(300, ErrorMessage = "پیام شما نباید بیشتر از 300 کاراکتر باشد")]
        public string? Response { get; set; }
       public IFormFile? Document { get; set; }
    }
}
