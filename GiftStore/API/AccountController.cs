
using api.View.User;
using Azure.Core;
using DocumentFormat.OpenXml.Office2016.Excel;
using GiftStore.Data;
using GiftStore.Models;
using GiftStore.Services;
using GiftStore.Services.Sms;
using GiftStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GiftStore.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public Db_API db { get; set; }

        private readonly IConfiguration _configuration;
        private readonly ISmsService _smsService;
        private readonly ILogger<TestController> _logger;

        public AccountController(Db_API db_, ISmsService smsService, ILogger<TestController> logger, IConfiguration configuration)
        {
           db = db_;
           
            _smsService = smsService;
            _logger = logger;
            _configuration =configuration;
        }
        
       
        [HttpPost("/AddContactUs")]
        public IActionResult AddContactUs([FromBody] AddContactUsViewModel model)
        {
            Contact_us newContact = new Contact_us
            {
              
                Text = model.Text,
                FullName = model.FullName,  
                Mobile= model.Mobile,
                Email= model.Email,
                SaleSection= model.SaleSection,

            };
            db.contact_Us.Add(newContact);
            db.SaveChanges();
            return Ok(new { Status = true, message = "پیشنهاد با موفقیت ثبت شد", newContact });

        }


        [HttpPost("/SendOtp")]
        public async Task<IActionResult> SendOtp([FromBody] SMSRequest request)
        {

            // اعتبارسنجی ورودی‌ها
            if (request == null || string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Message))
            {
                _logger.LogError("Validation error: Phone number or message is empty.");
                return BadRequest(new { error = "Phone number and message cannot be empty." });
            }

            try
            {

                string code = "" + GenerateRandomNumber(6);


                request.Message = code;
                request.TemplateType = "Verify";
                ////
                //SmsBank newOTPcode = new SmsBank();
                //newOTPcode.Code = code;
                //newOTPcode.phone = request.PhoneNumber;
                //db.smsBanks.Add(newOTPcode);
                //db.SaveChanges();
                var result = await _smsService.SendSmsAsync(request.PhoneNumber, request.Message, request.TemplateType);

                if (result)
                {
                    _logger.LogInformation($"SMS successfully sent to {request.PhoneNumber} using template {request.TemplateType}.");
                    return Ok(new { success = true, code,message = "SMS sent successfully." });
                }
                else
                {
                    _logger.LogWarning($"Failed to send SMS to {request.PhoneNumber} using template {request.TemplateType}.");
                    return StatusCode(500, new { error = "Failed to send SMS. Check the service logs for more details." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending SMS.");
                return StatusCode(500, new
                {
                    error = "An unexpected error occurred.",
                    details = ex.Message
                });


            }

        }


        [HttpPost("/Checkotp")]
        public IActionResult CheckOtp(string otpcode, string ?inputcode, string phone)
        {
            
            if (string.IsNullOrEmpty(inputcode))
             {
                return Ok(new
                {
                    message = "inccorect code!",
                    Status = false,

                });

            }
            
          




            var existingUser = db.users
                    .FirstOrDefault(u => u.Phone == phone);

                if (existingUser == null)
                {
                    if (otpcode==inputcode)
                    {

                        // ایجاد کاربر جدید
                        var newUser = new Users
                        {
                            Role = "user",
                            wallet = 0,
                            Stars = 0,
                            Active = true,
                            RegisterDate = DateTime.Now,
                            //Password = passwordHash,
                            //Email = registerUserDto.Email,
                            //Username = registerUserDto.Username,
                            Phone = phone
                        };

                        // ذخیره کاربر در پایگاه داده
                        db.users.Add(newUser);
                        db.SaveChanges();


                        var token = GenerateJwtToken(newUser);

                        return Ok(new

                        {
                            Status = true,
                            UserData = new
                            {
                                newUser
                            },
                            Token = token
                        });



                    }
                    else
                    {
                        return Ok(new
                        {
                            message = "inccorect code!",
                            Status = false,

                        });




                    }




                }
                else
                {
                if (otpcode == inputcode)
                {

                    
                    
                    var token = GenerateJwtToken(existingUser);

                    return Ok(new
                    {
                        Status = true,
                        
                        Token = token
                    });



                }
                else
                {
                    return Ok(new
                    {
                        message = "inccorect code!",
                        Status = false,

                    });




                }



            }


        } 


        public static long GenerateRandomNumber(int x)
        {
            if (x <= 0)
            {
                throw new ArgumentException("The length of the number must be greater than 0.");
            }

            Random random = new Random();
            long minValue = (long)Math.Pow(10, x - 1);
            long maxValue = (long)Math.Pow(10, x) - 1;

            return random.Next((int)minValue, (int)maxValue + 1);
        }

        //[HttpPost("/LoginOrRegisterUser")]
        //public IActionResult LoginOrRegisterUser(string phone)
        //{
        //    // اعتبارسنجی ورودی‌ها
        //    if (string.IsNullOrWhiteSpace(phone)
        //      )
        //    {
        //        return BadRequest(new { Status = false, message = "اطلاعات را کامل وارد کنید" });
        //    }


        //    // بررسی وجود کاربر با نام کاربری یا ایمیل مشابه
        //    var existingUser = db.users
        //        .FirstOrDefault(u => u.Phone == phone);

        //    if (existingUser == null)
        //    {




        //        // ایجاد کاربر جدید
        //        var newUser = new Users
        //        {
        //            Role = "user",
        //            wallet = 0,
        //            Stars = 0,
        //            Active = true,
        //            RegisterDate = DateTime.Now,
        //            //Password = passwordHash,
        //            //Email = registerUserDto.Email,
        //            //Username = registerUserDto.Username,
        //            Phone = phone
        //        };

        //        // ذخیره کاربر در پایگاه داده
        //        db.users.Add(newUser);
        //        db.SaveChanges();


        //       return BadRequest(new { Status = false, Message = "کاربری با این شماره یا ایمیل موجود میباشد" });
        //    }

         
          

        

        //    // تولید توکن JWT

        //    var user1 = db.users
        //        .FirstOrDefault(u => u.Email == newUser.Email);

        //    var token = GenerateJwtToken(user1);

        //    return Ok(new
        //    {
        //        Status = true,
        //        UserData = new
        //        {
        //            Username = newUser.Phone,
        //            Email = newUser.Email
        //        },
        //        Token = token
        //    });
        //}


        [HttpPost("/RegisterUser")]
        public  IActionResult RegisterUser([FromBody] UserRegistrationDTO registerUserDto)
        {
            // اعتبارسنجی ورودی‌ها
            if (registerUserDto == null || string.IsNullOrWhiteSpace(registerUserDto.Email) ||
                string.IsNullOrWhiteSpace(registerUserDto.Password) || string.IsNullOrWhiteSpace(registerUserDto.Email))
            {
                return BadRequest(new { Status = false,message="اطلاعات را کامل وارد کنید" });
            }


            // بررسی وجود کاربر با نام کاربری یا ایمیل مشابه
            var existingUser =  db.users
                .FirstOrDefault(u => u.Email == registerUserDto.Email || u.Phone == registerUserDto.PhoneNumber);

            if (existingUser != null)
            {
                return BadRequest(new { Status = false, Message = "کاربری با این شماره یا ایمیل موجود میباشد" });
            }

            // هش کردن پسورد
            var passwordHash = HashPassword(registerUserDto.Password);

            // ایجاد کاربر جدید
            var newUser = new Users
            {
                Role = "user",
                wallet = 0,
                Stars = 0,
                Active = true,
                RegisterDate = DateTime.Now,
                Password = passwordHash,
                Email = registerUserDto.Email,
                Username = registerUserDto.Username,
                Phone = registerUserDto.PhoneNumber
            };

            // ذخیره کاربر در پایگاه داده
            db.users.Add(newUser);
             db.SaveChanges();

            // تولید توکن JWT

            var user1 = db.users
                .FirstOrDefault(u => u.Email == newUser.Email);

            var token = GenerateJwtToken(user1);

            return Ok(new
            {
                Status = true,
                UserData = new
                {
                    Username = newUser.Phone,
                    Email = newUser.Email
                },
                Token = token
            });
        }

        [HttpPost("/LoginUser")]
        public IActionResult LoginUser([FromBody] UserLoginViewModel loginUserDto)
        {
            // Input validation
            if (loginUserDto == null || string.IsNullOrWhiteSpace(loginUserDto.Email) ||
                string.IsNullOrWhiteSpace(loginUserDto.Password))
            {
                return BadRequest(new { Status = false,message = "اطلاعات را کامل وارد کنید" });
            }

            // Find user in the database
            var user =  db.users
                .FirstOrDefault(u => u.Email == loginUserDto.Email);

            if (user == null)
            {
                return BadRequest(new { Status = false, Message = "کاربری با این ایمیل موجود نیست" });
            }

            // Compare input password with hashed password in the database
            var passwordHash = HashPassword(loginUserDto.Password);
            if (user.Password != passwordHash)
            {
                return BadRequest(new { Status = false, Message = "پسورد اشتباه میباشد لطفا دوباره تلاش کنید" });
            }

            // Successful login
            var token = GenerateJwtToken(user);

            // ایجاد کاربر جدید
            var newLog = new ActivityLog
            {
                LogDate = DateTime.Now,
                UserId = user.Id,
                Description = "ورود",
                UserRole=user.Role,
                Name=user.Username
        
            };

            // ذخیره کاربر در پایگاه داده
            db.activityLogs.Add(newLog);
            db.SaveChanges();
            return Ok(new
            {
                Status =true,
                UserData = new
                {
                    LastName = user.LastName,
                    Email = user.Email
                },
                Token = token
            });
        }


        private string GenerateJwtToken(Users user)
        {
            var claims = new[]
            {
      
        new Claim("Email", user.Email?? string.Empty),
        new Claim("PhoneNumber", user.Phone ?? string.Empty),
         new Claim("Username", user.Username ?? string.Empty),
         new Claim("Role", user.Role ?? string.Empty),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // اضافه کردن شناسه کاربر
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }


        [HttpGet("/GetUserDetailsFromToken")]
        public IActionResult
            GetUserDetailsFromToken()
        {
            // دریافت توکن از هدر درخواست
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { StatusCode = 401, Message = "Token is missing or invalid." });
            }

            try
            {
                // تحلیل توکن و استخراج اطلاعات کاربر از آن
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                // استخراج اطلاعات کاربر از توکن
                var userClaims = jsonToken?.Claims.ToDictionary(c => c.Type, c => c.Value);

                if (userClaims == null || !userClaims.ContainsKey(ClaimTypes.NameIdentifier))
                {
                    return Unauthorized(new { StatusCode = 401, Message = "Invalid token." });
                }
                
                // ساخت اطلاعات کاربر به صورت DTO
                var userDetails = new
                {
                    Id = userClaims[ClaimTypes.NameIdentifier],
                    FirstName = userClaims.ContainsKey("FirstName") ? userClaims["FirstName"] : null,
                    LastName = userClaims.ContainsKey("LastName") ? userClaims["LastName"] : null,
                    Username= userClaims.ContainsKey("Username") ? userClaims["Username"] : null,
                    Email = userClaims.ContainsKey("Email") ? userClaims["Email"] : null,
                    Role = userClaims.ContainsKey("Role") ? userClaims["Role"] : null,
                    PhoneNumber = userClaims.ContainsKey("PhoneNumber") ? userClaims["PhoneNumber"] : null,
                    Country = userClaims.ContainsKey("Country") ? userClaims["Country"] : null,
                };
                var user = db.users.FirstOrDefault(c => c.Id == int.Parse(userDetails.Id));
                if (user==null )
                {
                    return BadRequest(new { Status = false, message = "no user found" });
                }
                return Ok(new
                {
                    Status = true,
                    UserData = userDetails
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status =false, Message = "An error occurred while processing the token.", Error = ex.Message });
            }
        }

        [HttpGet("/CheckAdminOrNot")]
        public IActionResult CheckAdminOrNot()
        {
            // دریافت توکن از هدر درخواست
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { StatusCode = 401, Message = "Token is missing or invalid." });
            }

            try
            {
                // تحلیل توکن و استخراج اطلاعات کاربر از آن
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                // استخراج اطلاعات کاربر از توکن
                var userClaims = jsonToken?.Claims.ToDictionary(c => c.Type, c => c.Value);

                if (userClaims == null || !userClaims.ContainsKey(ClaimTypes.NameIdentifier))
                {
                    return Unauthorized(new { StatusCode = 401, Message = "Invalid token." });
                }

                // ساخت اطلاعات کاربر به صورت DTO
                var userDetails = new
                {
                    Id = userClaims[ClaimTypes.NameIdentifier],
                    FirstName = userClaims.ContainsKey("FirstName") ? userClaims["FirstName"] : null,
                   
                    Role = userClaims.ContainsKey("Role") ? userClaims["Role"] : null,
                    PhoneNumber = userClaims.ContainsKey("PhoneNumber") ? userClaims["PhoneNumber"] : null,
                
                };
                var user = db.users.FirstOrDefault(c => c.Id == int.Parse(userDetails.Id));

                if (user == null)
                {
                    return BadRequest(new { Status = false, message = "no user found" });
                }
                if (userDetails.Role == "مدیر")
                {
                    return Ok(new
                    {
                        Status = true,
                        admin = true
                    });
                }
                else
                {
                    
                    return Ok(new
                    {
                        Status = true,
                        admin = false
                    });
                }
               
              
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = false, Message = "An error occurred while processing the token.", Error = ex.Message });
            }
        }

        

    }
}
