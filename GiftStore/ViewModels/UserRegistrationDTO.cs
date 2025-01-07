using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.View.User
{
    public class UserRegistrationDTO
    {




       // [Required(ErrorMessage = "Password is required.")]
       // [StringLength(100, ErrorMessage = "Password must be at least 6 characters long.", MinimumLength = 6)]
        public string Password { get; set; }

 

      //  [Required(ErrorMessage = "Phone number is required.")]
      //  [Phone(ErrorMessage = "The entered phone number is not valid.")]
        public string PhoneNumber { get; set; }


     //   [Required(ErrorMessage = "Username is required.")]
      //  [StringLength(50, ErrorMessage = "FirstName must be between 5 and 50 characters long.", MinimumLength = 5)]
        public string Username { get; set; }

      //  [Required(ErrorMessage = "Email is required.")]
       // [EmailAddress(ErrorMessage = "The entered email is not valid.")]
        public string Email { get; set; }

    

    }
}