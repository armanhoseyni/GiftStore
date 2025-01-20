namespace GiftStore.ViewModels
{
    public class AddNotificationViewModel
    {

        public string Title { get; set; }
        public string? ShowTo { get; set; }
        public string? Description { get; set; }
        public IFormFile? Document { get; set; }

        public string? Type { get; set; }
    }
}
