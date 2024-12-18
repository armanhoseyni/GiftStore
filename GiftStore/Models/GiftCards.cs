using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class GiftCards
    {

        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Country { get; set; }
        public DateTime ExpDate { get; set; }

        public double Price { get; set; }
 
        public string? Status { get; set; }
        public string? label { get; set; }
        public string? type { get; set; }
      

    }
}
