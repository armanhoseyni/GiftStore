using API_M.Models;
using GiftStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GiftStore.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPanelController : ControllerBase
    {
        public Db_API db { get; set; }
        public AdminPanelController(Db_API db_)
        {
            db = db_;
        }
        [HttpGet("/GetAllUsers")]
        public IActionResult GetAllUsers()
        {


            List<Users> products = db.users.ToList();
            return Ok(products);

        }
        [HttpPost("/AddUser")]
        public IActionResult AddUser(string firstName, string lastName, string phoneNumber, string? password, string? rePassword)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return BadRequest("Phone number is required.");

            if (password != rePassword)
                return BadRequest("Passwords do not match.");

            // Create a new user object
            var user = new Users
            {
                FirstName = firstName,
                LastName = lastName,
                Phone = phoneNumber,
                Password = password,
                RePassword = rePassword
            };

            // Add the user to the database
            db.users.Add(user);
            db.SaveChanges();

            return Ok(new { Message = "User added successfully.", User = user });
        }

        [HttpPut("/UpdateUser")]
        public IActionResult UpdateUser(int id, string? firstName, string? lastName, string? phone, string? password, string? rePassword)
        {
            var user = db.users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound("User not found.");

            if (!string.IsNullOrWhiteSpace(firstName))
                user.FirstName = firstName;

            if (!string.IsNullOrWhiteSpace(lastName))
                user.LastName = lastName;

            if (!string.IsNullOrWhiteSpace(phone))
                user.Phone = phone;

            if (!string.IsNullOrWhiteSpace(password) && password == rePassword)
                user.Password = password;

            db.SaveChanges();

            return Ok(new { Message = "User updated successfully.", User = user });
        }
        [HttpDelete("/DeleteUser")]
        public IActionResult DeleteUser(int id)
        {
            var user = db.users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound("User not found.");

            db.users.Remove(user);
            db.SaveChanges();

            return Ok(new { Message = "User deleted successfully." });
        }
        [HttpGet("/GetUserById")]
        public IActionResult GetUserById(int id)
        {
            var user = db.users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        [HttpGet("/SearchUsersByPhone")]
        public IActionResult SearchUsersByPhone(string phone)
        {
            var users = db.users.Where(u => u.Phone.Contains(phone)).ToList();
            if (!users.Any())
                return NotFound("No users found with the given phone number.");

            return Ok(users);
        }
        [HttpGet("/CheckUserExists")]
        public IActionResult CheckUserExists(string phone)
        {
            var exists = db.users.Any(u => u.Phone == phone);
            return Ok(new { Exists = exists });
        }


    }
}
