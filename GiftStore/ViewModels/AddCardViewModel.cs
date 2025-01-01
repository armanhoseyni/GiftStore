namespace GiftStore.ViewModels
{
    public class AddCardViewModel
    {
        public string Code { get; set; }

        public string Country { get; set; }
          public string? type { get; set; }
      
    
        public double Price { get; set; }

        public DateTime ExpDate { get; set; }
        public string? status { get; set; }

    }
}
