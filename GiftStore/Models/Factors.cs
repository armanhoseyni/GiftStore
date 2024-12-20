using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftStore.Models
{
    public class Factors
    {
        [Key]

        public int Id { get; set; }
        
        [ForeignKey("Users")]

        public int UserId { get; set; }
        public DateTime FactorDate { get; set; }
        [ForeignKey("GiftCards")]

        public int GiftId { get; set; }
       
        public double PaymentAmount { get; set; }

        public string Status { get; set; }
        public string Type { get; set; }
        public string UserPhone { get; set; }//for


    }
}
