using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class Contact_us
    {
        [Key]
        public int Id { get; set; }

        public string FullName{ get; set; }
        public string Mobile{ get; set; }
        public string Email{ get; set; }
        public string SaleSection{ get; set; }
        public string Text{ get; set; }

    }
}
