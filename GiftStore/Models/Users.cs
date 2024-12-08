using System.ComponentModel.DataAnnotations;

namespace GiftStore.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }
        public string? Password { get; set; }
        public string? RePassword { get; set; }
    }
}
