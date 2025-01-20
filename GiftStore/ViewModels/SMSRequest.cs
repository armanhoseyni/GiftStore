namespace GiftStore.ViewModels
{
    public class SMSRequest
    {
        public string PhoneNumber { get; set; }
        public string ?Message { get; set; }
        public string? TemplateType { get; set; }

    }
}
