using GiftStore.Migrations;
using System.ComponentModel.DataAnnotations;

namespace GiftStore.ViewModels
{
    public class UserLoginViewModel
    {
     //   [Required(ErrorMessage = "Email is required.")]
      //  [EmailAddress(ErrorMessage = "The entered email is not valid.")]
        public string Email { get; set; }

      //  [Required(ErrorMessage = "Password is required.")]
      //  [StringLength(100,ErrorMessage = "Password must be at least 6 characters long.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
