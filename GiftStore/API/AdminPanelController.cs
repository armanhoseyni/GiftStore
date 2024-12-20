using GiftStore.Data;
using GiftStore.Models;
using GiftStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using GiftStore.Services;
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

        //users
        [HttpGet("/GetAllUsers")]
        public IActionResult GetAllUsers()
        {


            List<Users> allusers = db.users.ToList();
            if (allusers.Count <= 0)
            {
                return NotFound(new { message = "No user found ", StatusCode = 404 });
            }
            else
            {


                return Ok(allusers);

            }

        }
        [HttpGet("/GetAllUsersCounts")]
        public IActionResult GetAllUsersCounts()
        {


            List<Users> allusers = db.users.Where(x=>x.Role=="User").ToList();
            if (allusers.Count <= 0)
            {
                return NotFound(new { message = "No user found ", StatusCode = 404 });
            }
            else
            {


                return Ok(allusers.Count);

            }

        }
        [HttpPost("/AddUser")]
        public IActionResult AddUser([FromBody] UsersViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Phone))
                return BadRequest("Phone number is required.");

            if (model.Password != model.RePassword)
                return BadRequest("Passwords do not match.");

            var user = new Users();
            if (db.users.Any(u => u.Phone == model.Phone))
            {
                return BadRequest("A user with this phone number exist");

            }
            else
            {/////
                ////hash passwords

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Phone = model.Phone;
                user.Password = model.Password;
                user.RePassword = model.RePassword;
                user.Email = model.Email;
                user.RegisterDate = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                user.Active = true;
                user.Stars = 0;
                user.wallet = 0;
                model.Role = "User";

                db.users.Add(user);
                db.SaveChanges();

                return Ok(new { Message = "User added successfully.", User = user });
            }

        }
 
        
        
        /// <summary>
        /// hash password
       
        [HttpPut("/UpdateUser")]
        public IActionResult UpdateUser([FromBody] UsersViewModel model)
        {
            var user = db.users.FirstOrDefault(u => u.Phone == model.Phone);

            if (!CheckUserExists1(model.Phone))
            {


                if (user == null)
                    return NotFound(new { message = "User not  found ", StatusCode = 404 });

                if (!string.IsNullOrWhiteSpace(model.FirstName))
                    user.FirstName = model.FirstName;

                if (!string.IsNullOrWhiteSpace(model.LastName))
                    user.LastName = model.LastName;

                if (!string.IsNullOrWhiteSpace(model.Phone))

                    user.Phone = model.Phone;

                if (!string.IsNullOrWhiteSpace(model.Role))

                    user.Role = model.Role;

                if (!string.IsNullOrWhiteSpace(model.Password) && model.Password == model.RePassword)
                    user.Password = model.Password;

                db.SaveChanges();

                return Ok(new { Message = "User updated successfully.", User = user });
            }
            else
            {
                return BadRequest("A user with this phone number exist");

            }

        }
        [HttpDelete("/DeleteUser")]
        public IActionResult DeleteUser(int id)
        {
            var user = db.users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { message = "User not  found ", StatusCode = 404 });

            db.users.Remove(user);
            db.SaveChanges();

            return Ok(new { Message = "User deleted successfully." });
        }
        [HttpGet("/GetUserById")]
        public IActionResult GetUserById(int id)
        {
            var user = db.users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { message = "User not  found ", StatusCode = 404 });

            return Ok(user);
        }

        [HttpGet("/SearchUsersByPhone")]
        public IActionResult SearchUsersByPhone(string phone)
        {
            var users = db.users.Where(u => u.Phone.Contains(phone)).ToList();
            if (!users.Any())
                return NotFound(new { message = "No users found with the given phone number.", StatusCode = 404 });

            return Ok(users);
        }
        [HttpGet("/CheckUserExists")]
        public IActionResult CheckUserExists(string phone)
        {
            var exists = db.users.Any(u => u.Phone == phone);
            if (!exists)
            {
                return NotFound(new { message = "No user exist with this phone nubmer", StatusCode = 404 });


            }
            return Ok(new { Exists = exists, StatusCode = 200 });
        }
        [HttpGet("/CheckUserExists1")]
        public bool CheckUserExists1(string phone)
        {
            var exists = db.users.Any(u => u.Phone == phone);
            if (!exists)
            {
                return false;

            }
            return true;
        }



        [HttpPut("/ActiveUser")]
        public IActionResult ActiveUser(string phone)
        {
            var user = db.users.FirstOrDefault(u => u.Phone == phone);
            if (user == null)
            {
                return NotFound(new { message = "کاربری با این شماره تلفن یافت نشد ", StatusCode = 404 });
            }
            else
            {


                user.Active = true;
                db.SaveChanges();
                return Ok(new { user, message = "کاربر فعال شد", StatusCode = 200 });

            }
        }

        [HttpPut("/DactiveUser")]
        public IActionResult DactiveUser(string phone)
        {
            var user = db.users.FirstOrDefault(u => u.Phone == phone);
            if (user == null)
            {
                return NotFound(new { message = "کاربری با این شماره تلفن یافت نشد ", StatusCode = 404 });
            }
            else
            {


                user.Active = false;
                db.SaveChanges();
                return Ok(new { user, message = "کاربر غیر فعال شد", StatusCode = 200 });

            }
        }

        [HttpGet("/ShowRespons")]
        public IActionResult ShowRespons(int id) 
        {
        var ResponsesTickets=db.tickets.Where(x=>x.UserId== id && x.Status=="پاسخ داده شده").ToList();
            if(ResponsesTickets.Count>0) { return Ok(ResponsesTickets); }
            else { return BadRequest(new { message="تیکتی پاسخ داده نشده" ,StatusCode=400}); }
      
        }
        [HttpGet("/ShowAllUsersTickets")]
        public IActionResult ShowAllUsersTickets(int id)
        {
            var ResponsesTickets = db.tickets.Where(x => x.UserId == id).ToList();
            if (ResponsesTickets.Count > 0) { return Ok(ResponsesTickets); }
            else { return BadRequest(new { message = "تیکتی موجود نمیباشد نشده", StatusCode = 400 }); }

        }


        [HttpPost("/CreateTicket")]
        public IActionResult CreateTicket([FromBody] AddTicketViewModel model)
        {
            var checkuser = db.users.FirstOrDefault(x => x.Id == model.UserId);
            if (checkuser != null)
            {
                Tickets newticket = new Tickets();
                newticket.Title = model.Title;
                newticket.Description = model.Description;
                newticket.Importance = model.Importance;
                newticket.SendDate = DateTime.Now;
                newticket.Status = "باز";
                newticket.UserId = model.UserId;
                //?  newticket.Document=model.Document;

                db.tickets.Add(newticket);
                db.SaveChanges();
                return Ok(new { message = "تیکت با موفقیت ثبت شد", newticket });
            }
            else
            {

                return BadRequest(new { message = "کاربری یافت نشد", StatusCode = 400 });
            }
        }

        //encrypt and decrypt
        [HttpGet("/GenerateKey")]
        public IActionResult GenerateKey()
        {
            Services.InfoSec en = new Services.InfoSec();
            string Key = en.GenerateKey();

            return Ok(Key);


        }

        [HttpGet("/CheckEncryption")]
        public IActionResult CheckEncryption(string text, string key)
        {
            Services.InfoSec en = new Services.InfoSec();
            string IVKey = "";
            var result = en.Encrypt(text, key, out IVKey);
            var IVKey1 = IVKey;
            return Ok(result + "/" + IVKey1);


        }

        [HttpGet("/Decryption")]

        public IActionResult Decryption(string ciper, string key, string IVKey)
        {
            Services.InfoSec en = new Services.InfoSec();

            var result = en.Decrypt(ciper, key, IVKey);

            return Ok(result);


        }





        ///gift cards
        [HttpPost("/AddGiftCart")]
        public IActionResult AddGiftCart(string code, string country, string label, string type, double price, DateTime expdate, string status, string directory)
        {
            Services.InfoSec en = new Services.InfoSec();
            string Key = en.GenerateKey();
            string IVKey = "";
            var result = en.Encrypt(code, Key, out IVKey);

            //create uniqe label

            var randomdigit = en.GenerateRandom10DigitNumber();

            var searchlabel = db.giftCards.Where(x => x.label == randomdigit).ToList();


            while (searchlabel.Any())
            {
                randomdigit=en.GenerateRandom10DigitNumber();
            }
            



            GiftCards newCard = new GiftCards
            {
                Code = result,
                Country = country,
                label = randomdigit,
                type = type,
                Price = price,
                ExpDate = expdate,
                Status = status
            };

            db.giftCards.Add(newCard);

            db.SaveChanges();
            // Save to Excel
            SaveToExcel(newCard.Id, code, result, Key, IVKey, directory);

            // Prepare the file for download
            string filePath = @"D:\c# codes\api5\GiftStore\GiftCards\GiftCards.xlsx";
            if (System.IO.File.Exists(filePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GiftCards.xlsx");
            }

            return Ok(new { message = "کارت با موفقیت ثبت شد ", StatusCode = 200 });

        }

        private void SaveToExcel(int id, string code, string result, string key, string ivKey, string directory)
        {
            string directoryPath = directory;
            string fileName = "GiftCards.xlsx";
            string filePath = Path.Combine(directoryPath, fileName);

            // Ensure the directory exists
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Initialize the workbook and worksheet
            IXLWorkbook workbook;
            IXLWorksheet worksheet;

            if (System.IO.File.Exists(filePath))
            {
                // Load the existing file
                workbook = new ClosedXML.Excel.XLWorkbook(filePath);
                worksheet = workbook.Worksheets.First();
            }
            else
            {
                // Create a new workbook and worksheet
                workbook = new ClosedXML.Excel.XLWorkbook();
                worksheet = workbook.Worksheets.Add("GiftCards");
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Code";
                worksheet.Cell(1, 3).Value = "Result";
                worksheet.Cell(1, 4).Value = "Key";
                worksheet.Cell(1, 5).Value = "IVKey";
            }

            // Find the next empty row
            int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
            int newRow = lastRow + 1;

            // Add the data
            worksheet.Cell(newRow, 1).Value = id;
            worksheet.Cell(newRow, 2).Value = code;
            worksheet.Cell(newRow, 3).Value = result;
            worksheet.Cell(newRow, 4).Value = key;
            worksheet.Cell(newRow, 5).Value = ivKey;

            // Save the workbook
            workbook.SaveAs(filePath);
        }



        [HttpGet("/GetAllGiftCards")]

        public IActionResult GetAllGiftCards()
        {


            List<GiftCards> allgiftcards = db.giftCards.ToList();
            if (allgiftcards.Count <= 0)
            {
                return NotFound(new { message = "No giftcard  found ", StatusCode = 404 });
            }
            else
            {


                          return Ok(allgiftcards);

             }
        }


        [HttpGet("/SearchGiftCardByStatus")]

        public IActionResult SearchGiftCardByStatus(string status)
        {
            var giftcards = db.giftCards.Where(u => u.Status == (status)).ToList();
            if (!giftcards.Any())
                return NotFound(new { message = "گیفت کارتی با چنین وضعیتی وجود ندارد", StatusCode = 404 });

            return Ok(giftcards);

        }


        [HttpGet("/SearchGiftCardByCountry")]

        public IActionResult SearchGiftCardByCountry(string country)
        {
            var giftcards = db.giftCards.Where(u => u.Country == (country)).ToList();
            if (!giftcards.Any())
                return NotFound(new { message = "گیفت کارتی با چنین کشوری وجود ندارد", StatusCode = 404 });

            return Ok(giftcards);

        }

        [HttpGet("/SearchGiftCardByType")]

        public IActionResult SearchGiftCardByType(string type)
        {
            var giftcards = db.giftCards.Where(u => u.type == (type)).ToList();
            if (!giftcards.Any())
                return NotFound(new { message = "گیفت کارتی با چنین نوعی وجود ندارد", StatusCode = 404 });

            return Ok(giftcards);

        }

        [HttpDelete("/DeleteGiftCard")]

        public IActionResult DeleteGiftCard(string label)
        {
            var Gc = db.giftCards.FirstOrDefault(u => u.Code == label);
            if (Gc == null)
                return NotFound(new { message = " گیفت کارت پیدا نشد ", StatusCode = 404 });

            db.giftCards.Remove(Gc);
            db.SaveChanges();

            return Ok(new { Message = "گیفت کارت با موفقیت حذف شد" });

        }

        [HttpPut("/UpdategiftCard")]
        public IActionResult UpdategiftCard(string code, string country, string label, string type, double price, DateTime expdate, string status)
        {
            var giftcard = db.giftCards.FirstOrDefault(u => u.label == label);

            if (giftcard == null)
            {
                return NotFound(new { mesasge = "گیفت کارتی با این لیبل یافت نشد", StatusCode = 404 });

            }
            //// save ivkey and key and ciper anywhere
            else
            {
                InfoSec en = new InfoSec();
                string IVkey = "";
                giftcard.Code = en.Encrypt(code,en.GenerateKey(),out IVkey);
                giftcard.Country = country;
                giftcard.type = type;
                giftcard.Price = price;
                giftcard.ExpDate = expdate;
                giftcard.Status = status;
                db.SaveChanges();
                return Ok(new {meessage="اطلاعات با موفقیت بروزرسانی شد ", StatusCode=200});


            }
        }

        [HttpGet("/SoldGiftsCount")]
        public IActionResult SoldGiftsCount()
        {
            var soldgiftcards = db.giftCards.Where(u => u.Status == ("فروخته شده")).ToList();
            if (soldgiftcards.Count <= 0)
            {
                return NotFound(new { message = "گیفت کارتی پیدا نشد ", StatusCode = 404 });
            }
            else
            {


                return Ok(soldgiftcards);

            }
        }













        //tickets

        //  - مشاهده درخواست‌های پشتیبانی ارسال شده توسط کاربر.
        [HttpGet("/ShowUserTickets")]
        public IActionResult ShowUserTickets(int id)
        {
            var tickets = db.tickets.Where(t => t.UserId==id).ToList();
            if (!tickets.Any())
            {
                return BadRequest(new { message = "درخواستی از طرف این کاربر وجود ندارد", StatusCode = 400 });

            }
            else{
                return Ok(tickets);
            }


        }
        [HttpGet("/GetAllTickets")]
        public IActionResult GetAllTickets()
        {
            var tickets = db.tickets.ToList();
            if (!tickets.Any())
            {
                return BadRequest(new { message = "درخواستی از طرف این کاربر وجود ندارد", StatusCode = 400 });

            }
            else
            {
                return Ok(tickets);
            }


        }



        [HttpGet("/GetAllTicketsByImportance")]
        public IActionResult GetAllTicketsByImportance()
        {
            var tickets = db.tickets.OrderByDescending(t => t.Importance).ToList();

            // Check if the tickets list is empty
            if (!tickets.Any())
            {
                return BadRequest(new { message = "درخواستی از طرف این کاربر وجود ندارد", StatusCode = 400 });
            }
            else
            {
                return Ok(tickets);
            }


        }

        [HttpGet("/GetAllTicketsByStatus")]

        public IActionResult GetAllTicketsByStatus(string status)
        {
            var tickets = db.tickets.Where(t => t.Status==status).ToList();

            // Check if the tickets list is empty
            if (!tickets.Any())
            {
                return BadRequest(new { message = "درخواستی از طرف این کاربر وجود ندارد", StatusCode = 400 });
            }
            else
            {
                return Ok(tickets);
            }


        }

        [HttpPut("/SendResponeToTickets")]

        public IActionResult SendResponeToTickets(int id,string response)
        {

            var ticket= db.tickets.FirstOrDefault(u => u.Id == id);
            if (ticket != null)
            {

            ticket.response=response;
            ticket.responseDate= DateTime.Now;
            ticket.Status= "پاسخ داده شده";
            
            db.SaveChanges();
                return Ok(new {message="تیکت با موفقیت پاسخ داده شد",StatusCode=200,ticket});
            }
            else
            {
                return BadRequest(new {message="تیکتی یافت نشد", StatusCode = 400 });
            }

               
            }
        }
    }
