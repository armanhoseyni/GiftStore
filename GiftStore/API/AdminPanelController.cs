using GiftStore.Data;
using GiftStore.Models;
using GiftStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using GiftStore.Services;
using System.Reflection.Emit;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using DocumentFormat.OpenXml.InkML;
using System.Text.Json.Serialization;
using System.Text.Json;


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
        //contact us
        [HttpGet("/GetAllContactUs")]
        public IActionResult GetAllContactUs()
        {
            var Allcontactus = db.contact_Us.ToList();
            if (Allcontactus.Count <= 0)
            {
                return NotFound(new { Status = false, Allcontactus});
            }
            return Ok(new { Status = true, Allcontactus });
        }
        // Users
        [HttpGet("/GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var allUsers = db.users.ToList();

            if (allUsers.Count <= 0)
            {
                return NotFound(new { Status = false,allUsers});
            }

            return Ok(new { Status = true,allUsers });
        }


        [HttpGet("/GetUserWalletLogs")]
        public IActionResult GetUserWalletLogs([FromQuery] int id)
        {
            var AllTransactions = db.walletLogs.Where(x=>x.UserId==id);
            if (AllTransactions.Count()==0)
            {
                return Ok(new { Status = false, AllTransactions });
            }
            return Ok(new {Status=true, AllTransactions });


        }

        [HttpGet("/GetAllUsersCounts")]
        public IActionResult GetAllUsersCounts()
        {
            var allusers = db.users.Where(x => x.Role == "User").ToList();
            if (allusers.Count <= 0)
            {
                return NotFound(new { Status = false ,allusers });
            }
            return Ok(new { Status = true, count = allusers.Count });
        }

        [HttpPost("/AddUser")]
        public IActionResult AddUser([FromBody] UsersViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Phone))
                return BadRequest(new { Status = false, message = "Phone number is required." });

            if (model.Password != model.RePassword)
                return BadRequest(new { Status = false, message = "Passwords do not match." });

            if (db.users.Any(u => u.Phone == model.Phone))
            {
                return BadRequest(new { Status = false, message = "A user with this phone number exists" });
            }

            var user = new Users
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,
                Password = model.Password,
                RePassword = model.RePassword,
                Email = model.Email,
                RegisterDate = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"),
                Active = true,
                Stars = 0,
                wallet = 0,
                Role = model.Role
            };

            db.users.Add(user);
            db.SaveChanges();

            return Ok(new { Status = true, message = "User added successfully.", User = user });
        }

        [HttpPost("/UpdateUser")]
        public IActionResult UpdateUser([FromBody] UpdateUserViewModel model, [FromQuery] string Phone)
        {
            var user = db.users.FirstOrDefault(u => u.Phone ==Phone);

            if (user == null)
                return NotFound(new { Status = false, message = "User not found", user });

         
            if (!string.IsNullOrWhiteSpace(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrWhiteSpace(model.LastName))
                user.LastName = model.LastName;
            if (!string.IsNullOrWhiteSpace(model.Email))
                user.Email = model.Email;


            if (!string.IsNullOrWhiteSpace(model.Role))
                user.Role = model.Role;

            
            db.SaveChanges();

            return Ok(new { Status = true, message = "User updated successfully.", User = user });
        }
        [HttpPost("/DeleteUser")]
        public IActionResult DeleteUser([FromQuery] int id)
        {
            var user = db.users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { Status = false, message = "User not found", user });

            db.users.Remove(user);
            db.SaveChanges();

            return Ok(new { Status = true, message = "User deleted successfully." });
        }
        [HttpGet("/GetUserById")]
        public IActionResult GetUserById([FromQuery] int id)
        {
            var user = db.users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new { Status = false, message = "User not found", user });

            return Ok(new { Status = true, user });
        }

        [HttpGet("/SearchUsersByPhone")]
        public IActionResult SearchUsersByPhone([FromQuery] string phone)
        {
            var users = db.users.Where(u => u.Phone.Contains(phone)).ToList();
            if (!users.Any())
                return NotFound(new { Status = false, message = "کاربری با این اطلاعات یافت نشد", users });

            return Ok(new { Status = true, users });
        }

        [HttpGet("/CheckUserExists")]
        public IActionResult CheckUserExists([FromQuery] string phone)
        {
            var exists = db.users.Any(u => u.Phone == phone);
            if (!exists)
            {
                return NotFound(new { Status = false, message = "No user exists with this phone number", exists });
            }
            return Ok(new { Status = true, Exists = exists, StatusCode = 200 });
        }

        [HttpGet("/CheckUserExists1")]
        public bool CheckUserExists1([FromQuery] string phone)
        {
            var exists = db.users.Any(u => u.Phone == phone);
            return exists;
        }

        [HttpPost("/ActiveUser")]
        public IActionResult ActiveOrDeactiveUser([FromQuery] string phone)
        {
            var user = db.users.FirstOrDefault(u => u.Phone == phone);
            if (user == null)
            {
                return NotFound(new { Status = false, message = "کاربری با این شماره تلفن یافت نشد", user });
            }

            user.Active = !user.Active;
            db.SaveChanges();

            string message = user.Active ? "کاربر فعال شد" : "کاربر غیر فعال شد";
            return Ok(new { Status = true, user, message, StatusCode = 200 });
        }

        [HttpGet("/ShowActiveOrDeactiveUsers")]
        public IActionResult ShowActiveOrDeactiveUsers([FromQuery] bool active)
        {
            var users = db.users.Where(u => u.Active == active).ToList();
            string message = active ? "کاربران فعال" : "کاربران غیر فعال";
            return Ok(new { Status = true, users, message, StatusCode = 200 });
        }

        [HttpGet("/ShowRespons")]
        public IActionResult ShowRespons([FromQuery] int id)
        {
            var ResponsesTickets = db.tickets.Where(x => x.UserId == id && x.Status == "پاسخ داده شده").ToList();
            if (ResponsesTickets.Count <= 0)
            {
                return BadRequest(new { Status = false, message = "تیکتی پاسخ داده نشده", StatusCode = 400 });
            }
            return Ok(new { Status = true, ResponsesTickets });
        }

        [HttpGet("/ShowAllUsersTickets")]
        public IActionResult ShowAllUsersTickets([FromQuery] int id)
        {
            var ResponsesTickets = db.tickets.Where(x => x.UserId == id).ToList();
            if (ResponsesTickets.Count <= 0)
            {
                return BadRequest(new { Status = false, message = "تیکتی موجود نمیباشد", StatusCode = 400 });
            }
            return Ok(new { Status = true, ResponsesTickets });
        }

        [HttpGet("/GenerateKey")]
        public IActionResult GenerateKey()
        {
            InfoSec en = new InfoSec();
            string Key = en.GenerateKey();
            return Ok(new { Status = true, Key });
        }

        [HttpGet("/CheckEncryption")]
        public IActionResult CheckEncryption([FromQuery] string text, [FromQuery] string key)
        {
            InfoSec en = new InfoSec();
            string IVKey = "";
            var result = en.Encrypt(text, key, out IVKey);
            return Ok(new { Status = true, result, IVKey });
        }

        [HttpGet("/Decryption")]
        public IActionResult Decryption([FromQuery] string ciper, [FromQuery] string key, [FromQuery] string IVKey)
        {
            InfoSec en = new InfoSec();
            var result = en.Decrypt(ciper, key, IVKey);
            return Ok(new { Status = true, result });
        }
        /// <summary>
        /// import from excel
        //[HttpPost("ImportDynamic")]
        //public async Task<IActionResult> ImportDynamic(string modelName, IFormFile file)
        //{
        //    if (string.IsNullOrEmpty(modelName))
        //    {
        //        return BadRequest(new { message = "Model name is required." });
        //    }

        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest(new { message = "No file uploaded." });
        //    }

        //    if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
        //    {
        //        return BadRequest(new { message = "Invalid file format. Please upload an Excel file." });
        //    }

        //    try
        //    {
        //        var modelType = AppDomain.CurrentDomain.GetAssemblies()
        //            .SelectMany(a => a.GetTypes())
        //            .FirstOrDefault(t => t.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase));

        //        if (modelType == null)
        //        {
        //            return BadRequest(new { message = $"Model '{modelName}' not found." });
        //        }

        //        var dbSetProperty = _context.GetType().GetProperties()
        //            .FirstOrDefault(p => p.PropertyType.IsGenericType &&
        //                                 p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
        //                                 p.PropertyType.GetGenericArguments()[0] == modelType);

        //        if (dbSetProperty == null)
        //        {
        //            return BadRequest(new { message = $"DbSet for model '{modelName}' not found." });
        //        }

        //        var dbSet = dbSetProperty.GetValue(_context) as dynamic;

        //        using (var stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);
        //            using (var package = new ExcelPackage(stream))
        //            {
        //                var worksheet = package.Workbook.Worksheets[0];
        //                var rowCount = worksheet.Dimension.Rows;

        //                var dataList = Activator.CreateInstance(typeof(List<>).MakeGenericType(modelType)) as IList;

        //                for (int row = 2; row <= rowCount; row++)
        //                {
        //                    var instance = Activator.CreateInstance(modelType);

        //                    foreach (var property in modelType.GetProperties())
        //                    {
        //                        var columnIndex = Array.FindIndex(worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns]
        //                            .Select(c => c.Text.Trim()) // حذف فضاهای خالی از نام ستون‌ها
        //                            .ToArray(), col => col.Equals(property.Name.Trim(), StringComparison.OrdinalIgnoreCase)) + 1;

        //                        if (columnIndex > 0)
        //                        {
        //                            var cellValue = worksheet.Cells[row, columnIndex].Text.Trim(); // حذف فضاهای خالی از مقادیر سلول‌ها









        //                            // اگر مقدار خالی است و ستون اجازه NULL را نمی‌دهد، می‌توانیم یک مقدار پیش‌فرض قرار دهیم
        //                            if (string.IsNullOrEmpty(cellValue))
        //                            {
        //                                // بررسی اینکه آیا نوع داده nullable است
        //                                var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
        //                                if (underlyingType == null)
        //                                {
        //                                    // نوع داده غیر nullable است، بنابراین مقدار پیش‌فرض قرار می‌دهیم
        //                                    if (property.PropertyType == typeof(string))
        //                                    {
        //                                        property.SetValue(instance, string.Empty); // برای رشته‌ها
        //                                    }
        //                                    else if (property.PropertyType == typeof(int))
        //                                    {
        //                                        property.SetValue(instance, 0); // برای اعداد صحیح
        //                                    }
        //                                    // می‌توانید برای انواع دیگر نیز مقدار پیش‌فرض قرار دهید
        //                                }
        //                            }
        //                            else
        //                            {
        //                                var convertedValue = Convert.ChangeType(cellValue, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        //                                property.SetValue(instance, convertedValue);
        //                            }
        //                        }
        //                    }

        //                    dataList.Add(instance);
        //                }

        //                foreach (var item in dataList)
        //                {
        //                    dbSet.Add((dynamic)item);
        //                }

        //                await _context.SaveChangesAsync();
        //            }
        //        }

        //        return Ok(new { message = "File imported successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error importing file", error = ex.Message });
        //    }
        //}


        [HttpPost("ImportGiftCards")]
        public IActionResult ImportGiftCards(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded." });
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
            {
                return BadRequest(new { message = "Invalid file format. Please upload an Excel file." });
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream); // Synchronous file copy
                    using (var package = new OfficeOpenXml.ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        if (worksheet.Dimension == null)
                        {
                            return BadRequest(new { message = "The Excel file is empty." });
                        }

                        var rowCount = worksheet.Dimension.Rows;

                        // Check if the Excel file has the required columns
                        var headers = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns]
                            .Select(c => c.Text.Trim())
                            .ToList();

                        var requiredHeaders = new List<string> { "Code", "Country", "type", "Price", "ExpDate", "status" };
                        if (!requiredHeaders.All(h => headers.Contains(h)))
                        {
                            return BadRequest(new { message = "Invalid Excel file format. Required columns are missing." });
                        }

                        // Process each row
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var code = worksheet.Cells[row, headers.IndexOf("Code") + 1].Text.Trim();
                            var country = worksheet.Cells[row, headers.IndexOf("Country") + 1].Text.Trim();
                            var type = worksheet.Cells[row, headers.IndexOf("type") + 1].Text.Trim();
                            var price = Convert.ToDouble(worksheet.Cells[row, headers.IndexOf("Price") + 1].Text.Trim());
                            var expDate = Convert.ToDateTime(worksheet.Cells[row, headers.IndexOf("ExpDate") + 1].Text.Trim());
                            var status = worksheet.Cells[row, headers.IndexOf("status") + 1].Text.Trim();

                            // Encrypt the code
                            InfoSec en = new InfoSec();
                            string key = en.GenerateKey();
                            string ivKey = "";
                            var encryptedCode = en.Encrypt(code, key, out ivKey);

                            // Generate a unique label
                            var randomDigit = en.GenerateRandom10DigitNumber();
                            while (db.giftCards.Any(x => x.label == randomDigit))
                            {
                                randomDigit = en.GenerateRandom10DigitNumber();
                            }

                            // Create and save the gift card
                            GiftCards newCard = new GiftCards
                            {
                                Code = encryptedCode,
                                Country = country,
                                label = randomDigit,
                                type = type,
                                Price = price,
                                ExpDate = expDate,
                                Status = status
                            };

                            db.giftCards.Add(newCard);
                            db.SaveChanges();

                            // Save to Excel
                            string directory = @"wwwroot/GiftCards/GiftCards.xlsx";
                            SaveToExcel(newCard.Id, randomDigit, encryptedCode, key, ivKey, directory);
                        }
                    }
                }

                // Return the updated Excel file
                string filePath = @"wwwroot/GiftCards/GiftCards.xlsx";
                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GiftCards.xlsx");
                }

                return Ok(new { Status = true, message = "Gift cards imported successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error importing gift cards", error = ex.Message });
            }
        }

        private void SaveToExcel(int id, string label, string result, string key, string ivKey, string directory)
        {
            string directoryPath = Path.GetDirectoryName(directory);
            string fileName = Path.GetFileName(directory);
            string filePath = Path.Combine(directoryPath, fileName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            IXLWorkbook workbook;
            IXLWorksheet worksheet;

            if (System.IO.File.Exists(filePath))
            {
                workbook = new ClosedXML.Excel.XLWorkbook(filePath);
                worksheet = workbook.Worksheets.First();
            }
            else
            {
                workbook = new ClosedXML.Excel.XLWorkbook();
                worksheet = workbook.Worksheets.Add("GiftCards");
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "label";
                worksheet.Cell(1, 3).Value = "Result";
                worksheet.Cell(1, 4).Value = "Key";
                worksheet.Cell(1, 5).Value = "IVKey";
            }

            int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
            int newRow = lastRow + 1;

            worksheet.Cell(newRow, 1).Value = id;
            worksheet.Cell(newRow, 2).Value = label;
            worksheet.Cell(newRow, 3).Value = result;
            worksheet.Cell(newRow, 4).Value = key;
            worksheet.Cell(newRow, 5).Value = ivKey;

            workbook.SaveAs(filePath);
        }
        

        [HttpPost("/AddGiftCart")]
        public IActionResult AddGiftCart([FromBody] AddCardViewModel model)
        {
            InfoSec en = new InfoSec();
            string Key = en.GenerateKey();
            string IVKey = "";
            var result = en.Encrypt(model.Code, Key, out IVKey);

            var randomdigit = en.GenerateRandom10DigitNumber();
            while (db.giftCards.Any(x => x.label == randomdigit))
            {
                randomdigit = en.GenerateRandom10DigitNumber();
            }

            GiftCards newCard = new GiftCards
            {
                Code = result,
                Country = model.Country,
                label = randomdigit,
                type = model.type,
                Price = model.Price,
                ExpDate = model.ExpDate,
                Status = model.status
            };

            db.giftCards.Add(newCard);
            db.SaveChanges();

            string directory = @"wwwroot/GiftCards/GiftCards.xlsx";
            SaveToExcel1(newCard.Id, randomdigit, result, Key, IVKey, directory);

            string filePath = @"wwwroot/GiftCards/GiftCards.xlsx";
            if (System.IO.File.Exists(filePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GiftCards.xlsx");
            }

            return Ok(new { Status = true, newCard, message = "کارت با موفقیت ثبت شد", StatusCode = 200 });
        }

        private void SaveToExcel1(int id, string label, string result, string key, string ivKey, string directory)
        {
            string directoryPath = directory;
            string fileName = "GiftCards.xlsx";
            string filePath = Path.Combine(directoryPath, fileName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            IXLWorkbook workbook;
            IXLWorksheet worksheet;

            if (System.IO.File.Exists(filePath))
            {
                workbook = new ClosedXML.Excel.XLWorkbook(filePath);
                worksheet = workbook.Worksheets.First();
            }
            else
            {
                workbook = new ClosedXML.Excel.XLWorkbook();
                worksheet = workbook.Worksheets.Add("GiftCards");
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "label";
                worksheet.Cell(1, 3).Value = "Result";
                worksheet.Cell(1, 4).Value = "Key";
                worksheet.Cell(1, 5).Value = "IVKey";
            }

            int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
            int newRow = lastRow + 1;

            worksheet.Cell(newRow, 1).Value = id;
            worksheet.Cell(newRow, 2).Value = label;
            worksheet.Cell(newRow, 3).Value = result;
            worksheet.Cell(newRow, 4).Value = key;
            worksheet.Cell(newRow, 5).Value = ivKey;

            workbook.SaveAs(filePath);
        }

        [HttpGet("/GetGiftCardByLabel")]
        public IActionResult GetGiftCardByLabel([FromQuery] string label)
        {
            string filePath = @"wwwroot/GiftCards/GiftCards.xlsx/GiftCards.xlsx";
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { Status = false, message = "Excel file not found.", StatusCode = 404 });
            }

            IXLWorkbook workbook;
            try
            {
                workbook = new ClosedXML.Excel.XLWorkbook(filePath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = false, message = "Error loading Excel file: " + ex.Message, StatusCode = 500 });
            }

            IXLWorksheet worksheet = workbook.Worksheets.First();
            var row = worksheet.RowsUsed().FirstOrDefault(r => r.Cell("B").GetValue<string>() == label);

            if (row == null)
            {
                return NotFound(new { Status = false, message = "Label not found in the Excel file.", StatusCode = 404 });
            }

            var giftCardInfo = new
            {
                ID = row.Cell("A").GetValue<int>(),
                Label = row.Cell("B").GetValue<string>(),
                Result = row.Cell("C").GetValue<string>(),
                Key = row.Cell("D").GetValue<string>(),
                IVKey = row.Cell("E").GetValue<string>()
            };

            InfoSec dec = new InfoSec();
            string decryptedCode;
            try
            {
                decryptedCode = dec.Decrypt(giftCardInfo.Result, giftCardInfo.Key, giftCardInfo.IVKey);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = false, message = "Error decrypting the code: " + ex.Message, StatusCode = 500 });
            }
            var selectedgiftcard = db.giftCards.Where(x => x.label ==label).FirstOrDefault();

            var giftCard = new
            {
                ID = giftCardInfo.ID,
                Label = giftCardInfo.Label,
                type = selectedgiftcard.type,
                price = selectedgiftcard.Price,
                country = selectedgiftcard.Country,
                status = selectedgiftcard.Status,
                DecryptedCode = decryptedCode,
                Code = giftCardInfo.Key,
                IVKey = giftCardInfo.IVKey
            };

            return Ok(new
            {
                giftCard,
            Status=true,


            });

        }

        [HttpGet("/GetAllGiftCards")]
        public IActionResult GetAllGiftCards()
        {
            List<GiftCards> allgiftcards = db.giftCards.ToList();
            if (allgiftcards.Count <= 0)
            {
                return NotFound(new { Status = false, message = "No giftcard found", allgiftcards });
            }
            return Ok(new { Status = true, allgiftcards });
        }

        [HttpGet("/SearchGift")]
        public IActionResult SearchGift([FromQuery] string? status, [FromQuery] string? country, [FromQuery] string? type)
        {
            var query = db.giftCards.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(u => u.Status == status);

            if (!string.IsNullOrEmpty(country))
                query = query.Where(u => u.Country == country);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(u => u.type == type);

            var giftcards = query.ToList();
            if (!giftcards.Any())
            {
                return NotFound(new { Status = false, message = "گیفت کارتی با چنین وضعیتی وجود ندارد", giftcards });
            }
            return Ok(new { Status = true, giftcards });
        }

        [HttpPost("/DeleteGiftCard")]
        public IActionResult DeleteGiftCard([FromQuery] string label)
        {
            var Gc = db.giftCards.FirstOrDefault(u => u.label == label);
            if (Gc == null)
                return NotFound(new { Status = false, message = "گیفت کارت پیدا نشد", Gc });

            if (DeleteRow(label))
            {
                db.giftCards.Remove(Gc);
                db.SaveChanges();
                return Ok(new { Status = true, message = "گیفت کارت با موفقیت حذف شد" });
            }
            return NotFound(new { Status = false, message = "خطا در حذف گیفت کارت" });
        }

        private bool DeleteRow(string label)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var _filePath = @"wwwroot/GiftCards/GiftCards.xlsx/GiftCards.xlsx";

            if (string.IsNullOrEmpty(label) || !System.IO.File.Exists(_filePath))
            {
                return false;
            }

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowToDelete = -1;

                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    if (worksheet.Cells[row, 2].Text == label)
                    {
                        rowToDelete = row;
                        break;
                    }
                }

                if (rowToDelete != -1)
                {
                    worksheet.DeleteRow(rowToDelete);
                    package.Save();
                    return true;
                }
                return false;
            }
        }

        [HttpPost("/UpdategiftCard")]
        public IActionResult UpdategiftCard([FromBody] AddCardViewModel model, [FromQuery] string label)
        {
            var giftcard = db.giftCards.FirstOrDefault(u => u.label == label);
            if (giftcard == null)
            {
                return NotFound(new { Status = false, message = "گیفت کارتی با این لیبل یافت نشد", giftcard });
            }

            InfoSec en = new InfoSec();
            string IVkey = "";
            string newKey = en.GenerateKey();
            string encryptedCode = en.Encrypt(model.Code, newKey, out IVkey);

            giftcard.Code = encryptedCode;
            giftcard.Country = model.Country;
            giftcard.type = model.type;
            giftcard.Price = model.Price;
            giftcard.ExpDate = model.ExpDate;
            giftcard.Status = model.status;

            db.SaveChanges();

            string directory = @"wwwroot/GiftCards/GiftCards.xlsx";
            UpdateExcel(label, encryptedCode, newKey, IVkey, directory);

            return Ok(new { Status = true, message = "اطلاعات با موفقیت بروزرسانی شد", StatusCode = 200 });
        }

        private void UpdateExcel(string label, string encryptedCode, string key, string ivKey, string directory)
        {
            string filePath = Path.Combine(directory, "GiftCards.xlsx");

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("Excel file not found.");
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                bool rowFound = false;

                for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                {
                    if (worksheet.Cell(row, 2).Value.ToString() == label)
                    {
                        worksheet.Cell(row, 3).Value = encryptedCode;
                        worksheet.Cell(row, 4).Value = key;
                        worksheet.Cell(row, 5).Value = ivKey;
                        rowFound = true;
                        break;
                    }
                }

                if (!rowFound)
                {
                    throw new Exception($"Row with label '{label}' not found in the Excel file.");
                }

                workbook.Save();
            }
        }

        [HttpGet("/SoldGiftsCount")]
        public IActionResult SoldGiftsCount()
        {
            var soldgiftcards = db.giftCards.Where(u => u.Status == "فروخته شده").ToList();
            if (soldgiftcards.Count <= 0)
            {
                return NotFound(new { Status = false, soldgiftcards});
            }
            return Ok(new { Status = true, soldgiftcards });
        }

        [HttpGet("/GiftcardCountrys")]
        public IActionResult GiftcardCountrys()
        {
            var countries = new List<string> { "Iran", "Turkey", "USA", "Germany" };
            return Ok(new { Status = true, countries });
        }

        [HttpGet("/GiftcardTypes")]
        public IActionResult GiftcardTypes()
        {
            var types = new List<string> { "1", "2", "3" };
            return Ok(new { Status = true, types });
        }

        [HttpGet("/GiftcardStatus")]
        public IActionResult GiftcardStatus()
        {
            var statuses = new List<string> { "فروخته شده", "در دسترس", "بسته شده" };
            return Ok(new { Status = true, statuses });
        }
        /// <summary>
        /// tickets
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/ShowUserTickets")]
        public IActionResult ShowUserTickets([FromQuery] int id)
        {
            var tickets = db.tickets.Where(t => t.UserId == id).ToList();
            if (!tickets.Any())
            {
                return BadRequest(new { Status = false, message = "درخواستی از طرف این کاربر وجود ندارد", StatusCode = 400 });
            }
            return Ok(new { Status = true, tickets });
        }
        [HttpGet("/ShowUserTicketChats")]
        public IActionResult ShowUserTicketChats([FromQuery] int id)
        {
            var chat = db.ticketChats.Where(t => t.TicketId == id).ToList();
            if (!chat.Any())
            {
                return BadRequest(new { Status = false,chat, message = "درخواستی از طرف این کاربر وجود ندارد", StatusCode = 400 });
            }
            return Ok(new { Status = true, chat });
        }
        [HttpGet("/GetImage")]
        public IActionResult GetImage([FromQuery] int id)
        {
            var user = db.tickets.FirstOrDefault(t => t.Id == id);
            if (user == null)
            {
                return BadRequest(new { Status = false, message = "چنین کاربری موجود نمیباشد", StatusCode = 400 });
            }

            string filePath = db.tickets.FirstOrDefault(x => x.Id == id).DocumentPath;
            if (filePath == null)
            {
                return BadRequest(new { Status = false, message = "فایلی موجود نمیباشد", StatusCode = 400 });
            }

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { Status = false, message = "File not found or path is invalid." });
            }

            string contentType = GetContentType(filePath);
            var fileStream = System.IO.File.OpenRead(filePath);
            return File(fileStream, contentType);
        }

        private string GetContentType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".tiff" => "image/tiff",
                ".tif" => "image/tiff",
                _ => "application/octet-stream"
            };
        }

        [HttpGet("/GetAllTickets")]
        public IActionResult GetAllTickets()
        {
            var tickets = db.tickets.ToList();
            if (!tickets.Any())
            {
                return BadRequest(new { Status = false, message = "درخواستی از طرف این کاربر وجود ندارد", StatusCode = 400 });
            }
            return Ok(new { Status = true, tickets });
        }

        [HttpGet("/GetAllTicketsByImportance")]
        public IActionResult GetAllTicketsByImportance()
        {
            var tickets = db.tickets.OrderByDescending(t => t.Importance).ToList();
            if (!tickets.Any())
            {
                return BadRequest(new { Status = false, message = "درخواستی از طرف این کاربر وجود ندارد", StatusCode = 400 });
            }
            return Ok(new { Status = true, tickets });
        }

        [HttpGet("/GetAllTicketsByStatus")]
        public IActionResult GetAllTicketsByStatus([FromQuery] string status)
        {
            var tickets = db.tickets.Where(t => t.Status == status).ToList();
            if (!tickets.Any())
            {
                return BadRequest(new { Status = false, message = "درخواستی از طرف این کاربر وجود ندارد", StatusCode = 400 });
            }
            return Ok(new { Status = true, tickets });
        }

        [HttpPost("/SendResponeToTickets")]
        public IActionResult SendResponeToTickets([FromQuery] int id, [FromQuery] string response)
        {
            var chat = db.tickets.FirstOrDefault(u => u.Id == id);
            if (chat == null)
            {
                return BadRequest(new { Status = false,chat, message = "تیکتی یافت نشد", StatusCode = 400 });
            }
            TicketChats newRes= new TicketChats();

            newRes.Sender = 1;
            newRes.SendDate = DateTime.Now;
            newRes.message=response;
            newRes.TicketId = id;
            db.ticketChats.Add(newRes);
            db.SaveChanges();
            return Ok(new { Status = true, message = "تیکت با موفقیت پاسخ داده شد", chat });
        }

        [HttpPost("/CloseTicket")]
        public IActionResult CloseTicket([FromQuery] int id)
        {
            var ticket = db.tickets.FirstOrDefault(u => u.Id == id);
            if (ticket == null)
            {
                return BadRequest(new { Status = false, ticket, message = "تیکتی یافت نشد", StatusCode = 400 });
            }

            ticket.Status = "بسته شده";
            db.SaveChanges();
            return Ok(new { Status = true, message = "تیکت با موفقیت پاسخ داده شد", ticket });
        }


      
        /////telegram stars
        ///
        [HttpPost("/AddMinStars")]
        public IActionResult AddMinStars([FromQuery] int stars)
        {
            if(stars > 0)
            {
                var model = new TelegramStars
                {
                    MinStars = stars,

                };

                db.telegramStars.Add(model);
                db.SaveChanges();
                return Ok(new { Status = true, model });

            }
            else
            {
                return Ok(new { Status = false,message="ستاره نمیتواند کمتر از صفر باشد" });


            }


        }

        [HttpPost("/AddStarsPerDollar")]
        public IActionResult AddStarsPerDollar([FromQuery] int stars)
        {
            if (stars > 0)
            {
                var model = new TelegramStars
                {
                    StarsPerADollar = stars,

                };

                db.telegramStars.Add(model);
                db.SaveChanges();
                return Ok(new { Status = true, model });

            }
            else
            {
                return Ok(new { Status = false, message = "ستاره نمیتواند کمتر از صفر باشد" });


            }


        }


        [HttpPost("/removeStars")]
        public IActionResult removeStars([FromQuery] int id)
        {
            
        var UserRequest=db.userStarsLogs.Where(x=>x.UserId == id).FirstOrDefault();
            if (UserRequest == null)
            {
                return BadRequest(new { Status = false, message = "درخاستی از طرف این کاربر یافت نشد" });
            }
            else
            {
                var lastRow = db.telegramStars
      .OrderByDescending(t => t.Id) // Sort by Id in descending order
    .FirstOrDefault(); // Get the first row (which has the highest Id)
                var minStars = lastRow.MinStars;

                if (UserRequest.Status == "Waiting")
                {
                    var user = db.users.FirstOrDefault(x => x.Id == id);
                    user.Stars = -minStars;
                    UserRequest.Status = "success";
                    db.SaveChanges();
                    return Ok(new { Status = true, message = "امتیازات این کاربر صفر شد" });
                }
                else
                {
                    return NotFound(new { Status = false, message = "به این درخاست قبلا رسیدگی شده است" });

                }
            }
        
        }

        [HttpPost("/AddStars")]
        public IActionResult AddStars([FromQuery] int id, [FromQuery] int addstars)
        {
          


            var user = db.users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                user.Stars = addstars;
                db.SaveChanges();

                var newlog = new UserStarsLog();


                newlog.Star = addstars;
                newlog.Type = "income";
                newlog.Status = "success";
                newlog.LogDate = DateTime.Now;
                newlog.UserId = id;





                db.userStarsLogs.Add(newlog);
                db.SaveChanges();

                return Ok(new { Status = true, message = "امتیازات این کاربر اضافه شد" });

            }



            else
            {
                return BadRequest(new { Status = false, message = "کاربر یافت نشد" });

            }

        }
        /////Factors
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/GetAllFactors")]
        public IActionResult GetAllFactors()
        {
            var factors = db.factors
                .Include(f => f.GiftCard) // Include the GiftCard navigation property
                .Select(f => new
                {
                    // Fields from the Factors table
                    FactorId = f.Id,
                    UserId = f.UserId,
                    FactorDate = f.FactorDate,
                    GiftId = f.GiftId,
                    Status = f.Status,
                    Type = f.Type,
                    TransActionNumber = f.TransActionNumber,
                    FactorPrice = f.FactorPrice,
                    // Price from the GiftCards table
                     // Handle null GiftCard
                })
                .ToList();

            return Ok(factors); // Return the list of factors
        }
        [HttpGet("sum-payments")]
        public IActionResult SumPayments()
        {
            DateTime now = DateTime.Now;
            DateTime startOfLastWeek = now.AddDays(-7);
            DateTime startOfLastMonth = now.AddMonths(-1);

            double sumLastWeek = db.factors
                .Where(f => f.FactorDate >= startOfLastWeek && f.FactorDate <= now)
                .Sum(f => f.GiftCard.Price);

            double sumLastMonth = db.factors
                .Where(f => f.FactorDate >= startOfLastMonth && f.FactorDate <= now)
                .Sum(f => f.GiftCard.Price);

            return Ok(new
            {
                Status = true,
                SumLastWeek = sumLastWeek,
                SumLastMonth = sumLastMonth
            });
        }


        [HttpGet("/GetIncomeData")]
        public IActionResult GetIncomeData()
        {
            // Get the current year
            int currentYear = DateTime.Now.Year;

            // Query to group by month and sum PaymentAmount for monthly income
            var monthlyIncome = db.factors
                .Where(f => f.FactorDate.Year == currentYear) // Filter by the current year
                .GroupBy(f => f.FactorDate.Month) // Group by month
                .Select(g => new
                {
                    Month = g.Key, // Month number (1 for January, 2 for February, etc.)
                    Sales = g.Sum(f => f.GiftCard.Price) // Sum of PaymentAmount for the month
                })
                .OrderBy(m => m.Month) // Order by month
                .ToList();

            // Convert month numbers to month names and format the result
            var monthlyIncomeResult = monthlyIncome.Select(m => new
            {
                name = new DateTime(currentYear, m.Month, 1).ToString("MMMM"), // Month name
                sales = m.Sales // Total income for the month
            });

            // Query to group factors by GiftCard type and sum PaymentAmount for gift card type percentages
            var giftCardTypeIncome = db.factors
                .GroupBy(f => f.Type) // Group by GiftCard type
                .Select(g => new
                {
                    name = "type" + g.Key, // Format the type name (e.g., "type1", "type2")
                    value = g.Sum(f => f.GiftCard.Price) // Sum of PaymentAmount for this type
                })
                .ToList();

            // Return both results in a unified response
            return Ok(new
            {
                Status = true,
                MonthlyIncome = monthlyIncomeResult,
                GiftCardTypePercentages = giftCardTypeIncome
            });
        }
    }
}