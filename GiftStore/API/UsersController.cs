using GiftStore.Data;
using GiftStore.Models;
using GiftStore.Services;
using GiftStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace GiftStore.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public Db_API db { get; set; }
        public UsersController(Db_API db_)
        {
            db = db_;
        }

        /// <summary>
        ///tickets
        /// </summary>
        [HttpPost("/CreateTicket")]
        public IActionResult CreateTicket(AddTicketViewModel model)
        {
            var path = "";
            Services.Documents documentsService = new Services.Documents();
            string filePath = @"wwwroot\Documents";
            var checkuser = db.users.FirstOrDefault(x => x.Id == model.UserId);

            if (!string.IsNullOrEmpty(documentsService.UploadFile(model.Document, filePath)))
            {
                path = documentsService.UploadFile(model.Document, filePath);
            }
            else
            {
                path = null;
            }
            if (checkuser == null)
            {
                return BadRequest(new { Status = false, message = "کاربری یافت نشد", StatusCode = 200 });
            }

            Tickets newticket = new Tickets
            {
                Title = model.Title,
                Description = model.Description,
                SendDate = DateTime.Now,
                Status = "باز",
                UserId = model.UserId,

                DocumentPath = path,
            };

            db.tickets.Add(newticket);
            db.SaveChanges();
            TicketChats newChat = new TicketChats
            {
                message = model.Description,

                SendDate = DateTime.Now,
                Sender = 0,
                TicketId = newticket.Id,
                DocumentPath = path
            };

            db.ticketChats.Add(newChat);
            db.SaveChanges();


            return Ok(new { Status = true, message = "تیکت با موفقیت ثبت شد", newticket });
        }
        [HttpPost("/UpdateTicketDocument")]
        public IActionResult UpdateTicketDocument(int id, IFormFile file)
        {
            // Find the ticket by ID
            var ticket = db.tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return BadRequest(new { Status = false, message = "تیکتی با این شناسه یافت نشد", StatusCode = 200 });
            }

            // Define the directory to save the file
            string filePath = @"wwwroot\Documents";

            // Upload the file and get the saved file path
            Services.Documents documentsService = new Services.Documents();
            string documentPath = documentsService.UploadFile(file, filePath);

            if (string.IsNullOrEmpty(documentPath))
            {
                return BadRequest(new { Status = false, message = "خطا در آپلود فایل", StatusCode = 200 });
            }
            var path = "";
            if (!string.IsNullOrEmpty(documentPath))
            {
                path = documentPath;
            }
            else
            {
                path = null;
            }
            // Update the DocumentPath field in the ticket
            ticket.DocumentPath = path;
            db.SaveChanges();

            return Ok(new { Status = true, message = "فایل با موفقیت آپلود شد و مسیر آن ذخیره شد", ticket });
        }

        [HttpPost("/UpdateTicketChatDocument")]
        public IActionResult UpdateTicketChatDocument(int id, IFormFile file)
        {
            // Find the ticket by ID
            var ticket = db.ticketChats.FirstOrDefault(t => t.TicketId == id);
            if (ticket == null)
            {
                return BadRequest(new { Status = false, message = "تیکتی با این شناسه یافت نشد", StatusCode = 200 });
            }

            // Define the directory to save the file
            string filePath = @"wwwroot\Documents";
            var path = "";
            // Upload the file and get the saved file path
            Services.Documents documentsService = new Services.Documents();
            string documentPath = documentsService.UploadFile(file, filePath);
            if (!string.IsNullOrEmpty(documentPath))
            {
                path = documentPath;
            }
            else
            {
                path = null;
            }
            if (string.IsNullOrEmpty(documentPath))
            {
                return BadRequest(new { Status = false, message = "خطا در آپلود فایل", StatusCode = 200 });
            }

            // Update the DocumentPath field in the ticket
            ticket.DocumentPath = path;
            db.SaveChanges();

            return Ok(new { Status = true, message = "فایل با موفقیت آپلود شد و مسیر آن ذخیره شد", ticket });
        }
        /// <summary>
        /// Get an image associated with a ticket
        /// </summary>
        [HttpGet("/get-image")]
        public IActionResult GetImage([FromQuery] int id)
        {
            Services.Documents documentsService = new Services.Documents();

            var image = db.tickets.FirstOrDefault(t => t.Id == id);
            if (image == null)
            {
                return Ok(new { Status = true, image, message = "چنین تیکتی موجود نمیباشد" });
            }

            string filePath = db.tickets.FirstOrDefault(x => x.Id == id).DocumentPath;
            if (filePath == null)
            {
                return Ok(new { Status = true, filePath, message = "فایلی موجود نمیباشد", StatusCode = 200 });
            }

            // Remove the "wwwroot" prefix and replace backslashes with forward slashes

            return Ok(new { File = "https://gifteto.net/" + filePath });
        }
        /// <summary>
        /// Determine the content type of a file
        /// </summary>

        //تیکت ها

        /// <summary>
        /// Get all tickets for a specific user
        /// </summary>
        /// 

        [HttpGet("/get-GetAllTickets")]
        public IActionResult GetAllTickets([FromQuery] int id)
        {
            var tickets = db.tickets.Where(x => x.UserId == id).OrderByDescending(x => x.Id);

            return Ok(new { Status = true, tickets });


        }
        [HttpGet("/get-GetAllTicketChats")]
        public IActionResult GetAllTicketChats([FromQuery] int id)
        {

            var chat = db.ticketChats.Where(x => x.TicketId == id).ToList();

            if (chat.Count <= 0)
            {
                return Ok(new { Status = true, message = "چتی از طرف این کاربر وجود ندارد", chat, StatusCode = 200 });
            }

            var ticketStatus1 = db.tickets
             .Where(t => t.Id == id)
             .Select(t => t.Status)
             .FirstOrDefault(); // Use FirstOrDefault to get a single value

            var ticketStatus = true;
            if (ticketStatus1 == "بسته شده")
            {
                ticketStatus = false;
            }

            return Ok(new { Status = true, ticketStatus, chat, StatusCode = 200 });
        }



        [HttpPost("/delete-image")]


        public IActionResult DeleteImage([FromQuery] int id)
        {
            // Find the ticket by ID
            var ticket = db.tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return BadRequest(new { Status = false, message = "چنین کاربری موجود نمیباشد", ticket });
            }

            // Get the file path from the ticket
            string filePath = ticket.DocumentPath;
            if (string.IsNullOrEmpty(filePath))
            {
                return BadRequest(new { Status = false, message = "فایلی موجود نمیباشد", StatusCode = 200 });
            }

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return BadRequest(new { Status = false, message = "File not found or path is invalid." });
            }

            try
            {
                // Delete the file
                System.IO.File.Delete(filePath);

                // Clear the DocumentPath field in the database
                ticket.DocumentPath = "";
                db.SaveChanges();

                return Ok(new { Status = true, message = "فایل با موفقیت حذف شد", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = true, message = "خطا در حذف فایل: " + ex.Message, StatusCode = 500 });
            }
        }

        [HttpPost("/SendMessageToTickets")]
        public IActionResult SendMessageToTickets([FromQuery] int id, ViewModelResponseToTickets model)
        {
            var chat = db.tickets.FirstOrDefault(u => u.Id == id);

            string filePath = @"wwwroot\Documents";

            // Upload the file and get the saved file path
            Services.Documents documentsService = new Services.Documents();
            string documentPath = documentsService.UploadFile(model.Document, filePath);

            if (chat == null)
            {
                return BadRequest(new { Status = false, chat, message = "تیکتی یافت نشد", StatusCode = 200 });
            }
            if (chat.Status != "بسته شده")
            {
                TicketChats newRes = new TicketChats();

                newRes.Sender = 0;
                newRes.SendDate = DateTime.Now;
                newRes.message = model.Response;
                newRes.TicketId = id;

                if (!string.IsNullOrEmpty(documentPath))
                {
                    newRes.DocumentPath = documentPath;
                }
                else
                {
                    newRes.DocumentPath = null;
                }

                db.ticketChats.Add(newRes);
                db.SaveChanges();
                return Ok(new { Status = true, newRes });



            }
            else
            {
                return BadRequest(new { Status = false, message = "چت بسته شده است" });
            }
        }

        /// <summary>
        /// ////داشبورد
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/GetDashbordInformatons")]
        public IActionResult GetDashbordInformatons([FromQuery] int id)
        {
            var user = db.users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return Ok(new { Status = true, message = "چنین کاربری موجود نمیباشد", StatusCode = 200 });
            }

            var wallet = user.wallet;
            var giftcardcount = db.factors.Where(x => x.UserId == id).Count();
            var stars = user.Stars;

            return Ok(new { Status = true, wallet, giftcardcount, stars });
        }
        /// <summary>
        /// /score
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 


        [HttpGet("/GetAllRequests")]
        public IActionResult GetAllRequests([FromQuery] int id)
        {
            var reqs = db.userStarsLogs.Where(x =>  x.UserId == id&&x.Type=="0").OrderByDescending(x => x.Id).ToList();
            return Ok(new { reqs, Status = true });


        }


        [HttpGet("/GetUserStars")]
        public IActionResult GetUserStars([FromQuery] int id)
        {
            var stars = db.users.Where(x => x.Id == id).Select(x => x.Stars);
            if (stars.Any())
            {
                return Ok(new { Status = true, stars });

            }
            else
            {
                return Ok(new { Status = true });


            }
        }
        [HttpGet("/GetUserStarsLog")]
        public IActionResult GetUserStarsLog([FromQuery] int id)
        {
            var logss = db.userStarsLogs.Where(x => x.UserId == id).Where(x=>x.Status=="success").OrderByDescending(x => x.Id);
            // Fetch factors for the given user ID

            // Create a list to store the modified factors with decrypted codes
            var SuccessdLogs = new List<object>();

            // Iterate through each factor
            foreach (var log in logss)
            {
                // Call the GetGiftCard method to decrypt the gift card code

                // Add the decrypted code to the factor object
                var seccesed = new
                {
                    UserId = log.UserId,
                    Star = log.Star,
                    Type = log.Type,
                    LogDate = log.LogDate,


                };

                // Add the modified factor to the list
                SuccessdLogs.Add(seccesed);
            }
            return Ok(new { Status = true, SuccessdLogs });
        }

        [HttpPost("/RequestWithdraw")]
        public IActionResult RequestWithdraw([FromQuery] int id)
        {
            var lastRow = db.telegramStars
.OrderByDescending(t => t.Id) // Sort by Id in descending order
.FirstOrDefault(); // Get the first row (which has the highest Id)
            var minStars = lastRow.MinStars;

            // Retrieve the user from the database
            var user = db.users.FirstOrDefault(x => x.Id == id);
            if (user.Stars > minStars)
            {


                if (user == null)
                {
                    return Ok(new { message = "کاربری پیدا نشد", Status = false });
                }

                // Create a new UserStarsLog entry
                var newLog = new UserStarsLog
                {
                    UserId = id,
                    Star = user.Stars, // Assuming the user's current stars are to be logged
                    Type = "0", // Assuming this is a withdrawal, so type is "out"
                    LogDate = DateTime.Now,
                    Status = "Waiting" ,
                    Username=user.Username// Initial status is "Waiting"
                };

                // Add the new log entry to the database
                db.userStarsLogs.Add(newLog);



                // Save changes to the database
                db.SaveChanges();

                return Ok(new { Status = true, newLog });
            }
            else
            {

                return BadRequest(new { Status = false, message = "تعداد ستاره های شماره کمتراز حد مجاز است", MinStarNeed = minStars });

            }

        }

        /// <summary>
        /// wallet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 





        [HttpGet("/GetUserWallet")]
        public IActionResult GetUserWallet([FromQuery] int id)
        {
            var dollar = db.users.Where(x => x.Id == id).Select(x => x.wallet).FirstOrDefault();

            return Ok(new { Status = true, dollar });


        }


        [HttpGet("/GetUserWalletlog")]
        public IActionResult GetUserWalletlog([FromQuery] int id)
        {
            var logss = db.walletLogs.Where(x => x.UserId == id).OrderByDescending(x => x.Id).ToList();

            return Ok(new { Status = true, logss });


        }

        /////

        /// <summary>
        /// orders
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/GetUserFactors")]
        public IActionResult GetUserFactors([FromQuery] int id)
        {
            // Fetch factors for the given user ID
            var factors = db.factors.Where(x => x.UserId == id).OrderByDescending(x => x.Id).ToList();

            // Create a list to store the modified factors with decrypted codes
            var factorsWithDecryptedCodes = new List<object>();

            // Iterate through each factor
            foreach (var factor in factors)
            {
                // Call the GetGiftCard method to decrypt the gift card code
                string decryptedCode = GetGiftCard(factor.GiftId);

                // Add the decrypted code to the factor object
                var factorWithDecryptedCode = new
                {
                    id = factor.Id,
                    userId = factor.UserId,
                    factorDate = factor.FactorDate,
                    giftId = factor.GiftId,
                    factorPrice = factor.FactorPrice,
                    userName = factor.UserName,
                    user = factor.User,
                    giftCard = factor.GiftCard,
                    status = factor.Status,
                    type = factor.Type,
                    transActionNumber = factor.TransActionNumber,
                    decryptedcode = decryptedCode // Add the decrypted code here
                };

                // Add the modified factor to the list
                factorsWithDecryptedCodes.Add(factorWithDecryptedCode);
            }

            // Return the response with the modified factors
            return Ok(new { Status = true, factors = factorsWithDecryptedCodes });
        }

        [HttpGet("/GetGiftCard/{id}")]
        public string GetGiftCard(int id)
        {
            try
            {
                string directory = @"wwwroot/GiftCards/GiftCards.xlsx";
                string filePath = Path.Combine(directory, "GiftCards.xlsx");

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return "Excel file not found.";
                }

                // Open the Excel file
                using (var workbook = new ClosedXML.Excel.XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Assuming the data is in the first sheet

                    // Skip the header row (if it exists) and find the row with the matching ID
                    var row = worksheet.RowsUsed().Skip(1) // Skip the header row
                                                  .FirstOrDefault(r =>
                                                  {
                                                      var cellValue = r.Cell(1).GetValue<string>();
                                                      return int.TryParse(cellValue, out int rowId) && rowId == id;
                                                  });

                    if (row == null)
                    {
                        return "Gift card not found;";
                    }

                    // Extract data from the row
                    string label = row.Cell(2).GetValue<string>();
                    string encryptedResult = row.Cell(3).GetValue<string>();
                    string key = row.Cell(4).GetValue<string>();
                    string ivKey = row.Cell(5).GetValue<string>();

                    // Decrypt the data
                    InfoSec en = new InfoSec();
                    string decryptedResult = en.Decrypt(encryptedResult, key, ivKey);

                    // Return the decrypted data
                    return decryptedResult
                        ;
                }
            }
            catch (Exception ex)
            {
                return "An error";
            }
        }



        ////profile
        ///
        [HttpGet("/GetUserActivityLogs")]
        public IActionResult GetUserActivityLogs([FromQuery] int id)
        {
            var activitylogs = db.activityLogs.Where(x => x.UserId == id && x.Description == "ورود" || x.Description == "خروج").OrderByDescending(x => x.Id).ToList();

            return Ok(new { Status = true, activitylogs });


        }
        [HttpGet("/GetUserInfos")]
        public IActionResult GetUserInfos([FromQuery] int id)
        {
            var userinfos = db.users.Where(x => x.Id == id);

            return Ok(new { Status = true, userinfos });


        }

        /// <summary>
        /// //پروفایل
        /// </summary>
        /// <param name="mdoel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("/UpdateUserInfos")]
        public IActionResult UpdateUserInfos([FromBody] UpdateUserProfileViewModel mdoel, [FromQuery] int id)
        {
            var userinfos = db.users.FirstOrDefault(x => x.Id == id);
            if (userinfos == null)
            {
                return Ok(new { Status = false, message = "کاربری پیدا نشد" });

            }
            userinfos.FirstName = mdoel.FirstName;
            userinfos.LastName = mdoel.LastName;
            userinfos.Phone = mdoel.Phone;
            userinfos.Email = mdoel.Email;
            db.SaveChanges();

            return Ok(new { Status = true, userinfos });



        }
        [HttpPost("/UpdateUserPass")]
        public IActionResult UpdateUserPass(string oldpass, string newpas, [FromQuery] int id)
        {
            var userinfos = db.users.FirstOrDefault(x => x.Id == id);
            if (userinfos == null)
            {
                return Ok(new { Status = false, message = "کاربری پیدا نشد" });

            }

            if (userinfos.Password == HashPassword(oldpass))
            {
                userinfos.Password = HashPassword(newpas);
                db.SaveChanges()
                ;
                return Ok(new { Status = true, userinfos });



            }

            else
            {
                return BadRequest(new { Status = false, message = "پسورد فعلی اشتباه میباشد", userinfos });

            }


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











        [HttpGet("/GetNotifications")]
        public IActionResult GetNotifications([FromQuery] int userid, int? Status, string? showto)
        {
            //اگر پیام همگانی بخاد بببینه 
            if (showto == "0")
            {
                var Notifs = db.notifications.Where(x => (x.showTo == "0")).OrderByDescending(x => x.Id).ToList();
                return Ok(new { Status = true, Notifs });

            }
            if (showto == "" + userid)
            {
                {
                    var Notifs = db.notifications.Where(x => (x.showTo == "" + userid) && x.Status == Status).OrderByDescending(x => x.Id).ToList();
                    return Ok(new { Status = true, Notifs });

                }
            }

            else
            {
                var Notifs = db.notifications.Where(x => (x.showTo == "" + userid||x.showTo=="0") && x.Status == Status).OrderByDescending(x => x.Id).ToList();
                return Ok(new { Status = true, Notifs });


            }

        }
        [HttpPost("/SeenNotifications")]
        public IActionResult SeenNotifications( int userid, int NotifID)
        {
            var PersonalNotifs = db.notifications.FirstOrDefault(x => x.showTo == "" + userid &&x.Id==NotifID );
            if (PersonalNotifs == null)
            {
                return BadRequest(new { Status = false,message="اعلانی یافت نشد" });

            }
            PersonalNotifs.Status = 1;
            db.SaveChanges();   
            return Ok(new { Status = true, PersonalNotifs});


        }
    }
}