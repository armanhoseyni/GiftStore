using System.ComponentModel.DataAnnotations;

namespace GiftStore.ViewModels
{
    public class ViewModelResponseToTickets
    {
        [MaxLength(300, ErrorMessage = "The response must not exceed 300 characters.")]
        public string Response { get; set; }
    }
}
