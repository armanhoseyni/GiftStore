using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class Roles
    {
        [Key]   
        public int Id { get; set; }


        public string RoleName { get; set; }
        public string sath{ get; set; }

    }
}
