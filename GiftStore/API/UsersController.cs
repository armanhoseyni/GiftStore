using GiftStore.Data;
using GiftStore.Models;
using GiftStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        /// Create a new ticket
        /// </summary>
        [HttpPost("/CreateTicket")]
        public IActionResult CreateTicket(AddTicketViewModel model)
        {
            Services.Documents documentsService = new Services.Documents();
            string filePath = @"wwwroot\Documents";
            var checkuser = db.users.FirstOrDefault(x => x.Id == model.UserId);

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
                DocumentPath = documentsService.UploadFile(model.Document, filePath)
            };

            db.tickets.Add(newticket);
            db.SaveChanges();
            TicketChats newChat = new TicketChats
            {
                message = model.Description,
                
                SendDate = DateTime.Now,
                Sender  =0,
               TicketId = newticket.Id,
                DocumentPath = documentsService.UploadFile(model.Document, filePath)
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

            // Update the DocumentPath field in the ticket
            ticket.DocumentPath = documentPath;
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
                return Ok(new { Status = false, message = "چنین کاربری موجود نمیباشد", image });
            }

            string filePath = db.tickets.FirstOrDefault(x => x.Id == id).DocumentPath;
            if (filePath == null)
            {
                return BadRequest(new { Status = false, message = "فایلی موجود نمیباشد", StatusCode = 200 });
            }

           
           // string contentType = documentsService.GetContentType(filePath);
            //var fileStream = System.IO.File.OpenRead(filePath);
            return Ok(new {File= filePath });
        }

        /// <summary>
        /// Determine the content type of a file
        /// </summary>
       

        /// <summary>
        /// Get all tickets for a specific user
        /// </summary>
        [HttpGet("/get-GetAllTickets")]
        public IActionResult GetAllTickets([FromQuery] int id)
        {
            var user = db.users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return BadRequest(new { Status = false, message = "چنین کاربری موجود نمیباشد", StatusCode = 200 });
            }

            var tickets = db.tickets.Where(x => x.UserId == id).ToList();
            if (tickets.Count <= 0)
            {
                return Ok(new { Status = true, message = "درخواستی از طرف این کاربر وجود ندارد", StatusCode = 200 });
            }

            return Ok(new { Status = true, tickets, StatusCode = 200 });
        }
        [HttpGet("/get-GetAllTicketChats")]
        public IActionResult GetAllTicketChats([FromQuery] int id)
        {
          
            var chat = db.ticketChats.Where(x => x.TicketId == id).ToList();
            if (chat.Count <= 0)
            {
                return Ok(new { Status = true, message = "چتی از طرف این کاربر وجود ندارد",chat, StatusCode = 200 });
            }

            return Ok(new { Status = true, chat, StatusCode = 200 });
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
                return Ok(new { Status = false, message = "File not found or path is invalid." });
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
                return StatusCode(500, new { Status = false, message = "خطا در حذف فایل: " + ex.Message, StatusCode = 500 });
            }
        }


        [HttpGet("/GetDashbordInformatons")]
        public IActionResult GetDashbordInformatons([FromQuery] int id)
        {
            var user = db.users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return BadRequest(new { Status = false, message = "چنین کاربری موجود نمیباشد", StatusCode = 200 });
            }

            var wallet = user.wallet;
            var giftcardcount=db.factors.Where(x=> x.UserId == id).Count();
            var stars = user.Stars;
          
            return Ok(new { Status = true, wallet,giftcardcount,stars});
        }

        [HttpGet("/GetUserStars")]
        public IActionResult GetUserStars([FromQuery] int id)
        {
            var stars = db.users.Where(x=>x.Id== id).Select(x=>x.Stars);
            if (stars.Any())
            {
                return Ok(new { Status = true, stars });

            }
            else
            {
                return Ok(new { Status = false});


            }
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
            if (user.Stars > minStars) {


                if (user == null)
                {
                    return Ok("User not found");
                }

                // Create a new UserStarsLog entry
                var newLog = new UserStarsLog
                {
                    UserId = id,
                    Star = user.Stars, // Assuming the user's current stars are to be logged
                    Type = "out", // Assuming this is a withdrawal, so type is "out"
                    LogDate = DateTime.Now,
                    Status = "Waiting" // Initial status is "Waiting"
                };

                // Add the new log entry to the database
                db.userStarsLogs.Add(newLog);



                // Save changes to the database
                db.SaveChanges();

                return Ok(new { Status=true,newLog });
            }
            else
            {

                return Ok(new {Status=false,message="تعداد ستاره های شماره کمتراز حد مجاز است",MinStarNeed=minStars});

            }

        }

    }
}