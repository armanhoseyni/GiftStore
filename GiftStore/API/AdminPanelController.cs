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
using System.Globalization;
using GiftStore.Migrations;
using Polly;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace GiftStore.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPanelController : ControllerBase
    {
        public Db_API db { get; set; }
        private readonly IConfiguration _configuration;

        public AdminPanelController(Db_API db_, IConfiguration configuration)
        {
            db = db_;
            _configuration = configuration;
        }
        //contact us
        [HttpGet("/GetAllContactUs")]
        public IActionResult GetAllContactUs()
        {
            var Allcontactus = db.contact_Us.ToList();

            return Ok(new { Status = true, Allcontactus });
        }
        // Users
        [HttpGet("/GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var allUsers = db.users
       .OrderByDescending(u => u.Id) // Replace 'Id' with the field you want to sort by
       .ToList();

            return Ok(new { Status = true, allUsers });
        }


        [HttpGet("/GetUserWalletLogs")]
        public IActionResult GetUserWalletLogs([FromQuery] int id)
        {
            var AllTransactions = db.walletLogs.Where(x => x.UserId == id);

            return Ok(new { Status = true, AllTransactions });


        }

        [HttpGet("/GetAllUsersCounts")]
        public IActionResult GetAllUsersCounts()
        {
            var allusers = db.users.Where(x => x.Role == "User").ToList();

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
                RegisterDate = DateTime.Now,
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
            var user = db.users.FirstOrDefault(u => u.Phone == Phone);

            if (user == null)
                return BadRequest(new { Status = false, message = "User not found", user });


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
                return BadRequest(new { Status = false, message = "User not found", user });

            db.users.Remove(user);
            db.SaveChanges();

            return Ok(new { Status = true, message = "User deleted successfully." });
        }
        [HttpGet("/GetUserById")]
        public IActionResult GetUserById([FromQuery] int id)
        {
            var user = db.users.FirstOrDefault(u => u.Id == id);

            return Ok(new { Status = true, user });
        }

        [HttpGet("/SearchUsersByPhone")]
        public IActionResult SearchUsersByPhone([FromQuery] string? phone)
        {
            // Start with the base query
            var query = db.users.AsQueryable();

            // Apply phone filter if phone is provided
            if (!string.IsNullOrEmpty(phone))
            {
                query = query.Where(u => u.Phone.Contains(phone));
            }

            // Order the results by Id in descending order
            query = query.OrderByDescending(u => u.Id);

            // Execute the query and get the results
            var users = query.ToList();

            // Return the results
            return Ok(new { Status = true, users });
        }
        [HttpGet("/CheckUserExists")]
        public IActionResult CheckUserExists([FromQuery] string phone)
        {
            var exists = db.users.Any(u => u.Phone == phone);

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
                return Ok(new { Status = false, message = "کاربری با این شماره تلفن یافت نشد", user });
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

            return Ok(new { Status = true, ResponsesTickets });
        }

        [HttpGet("/ShowAllUsersTickets")]
        public IActionResult ShowAllUsersTickets([FromQuery] int id)
        {
            var ResponsesTickets = db.tickets.Where(x => x.UserId == id).ToList();

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
                return BadRequest(new { Status = false, message = "No file uploaded." });
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
            {
                return BadRequest(new { Status = false, message = "Invalid file format. Please upload an Excel file." });
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
                            return Ok(new { Status = true, message = "The Excel file is empty." });
                        }

                        var rowCount = worksheet.Dimension.Rows;

                        // Check if the Excel file has the required columns
                        var headers = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns]
                            .Select(c => c.Text.Trim())
                            .ToList();

                        // Add "addDate" to the list of required headers
                        var requiredHeaders = new List<string> { "Code", "Country", "type", "Price", "ExpDate", "status", "addDate" };
                        if (!requiredHeaders.All(h => headers.Contains(h)))
                        {
                            return Ok(new { message = "Invalid Excel file format. Required columns are missing." });
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
                            var addDate = Convert.ToDateTime(worksheet.Cells[row, headers.IndexOf("addDate") + 1].Text.Trim());

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
                                Status = status,
                                AddDate = addDate // Add the new field
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
            string directoryPath = directory; // Treat 'directory' as a directory path
            string fileName = "GiftCards.xlsx"; // Use a fixed file name
            string filePath = Path.Combine(directoryPath, fileName); // Combine directory and file name

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
                worksheet.Cell(1, 6).Value = "AddDate"; // Add the new column header
            }

            int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
            int newRow = lastRow + 1;

            worksheet.Cell(newRow, 1).Value = id;
            worksheet.Cell(newRow, 2).Value = label;
            worksheet.Cell(newRow, 3).Value = result;
            worksheet.Cell(newRow, 4).Value = key;
            worksheet.Cell(newRow, 5).Value = ivKey;
            worksheet.Cell(newRow, 6).Value = DateTime.Now.ToString("yyyy-MM-dd"); // Add the current date

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
                AddDate = model.AddDate,
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
                return Ok(new { Status = true, message = "Excel file not found.", StatusCode = 404 });
            }

            IXLWorkbook workbook;
            try
            {
                workbook = new ClosedXML.Excel.XLWorkbook(filePath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = true, message = "Error loading Excel file: " + ex.Message, StatusCode = 500 });
            }

            IXLWorksheet worksheet = workbook.Worksheets.First();
            var row = worksheet.RowsUsed().FirstOrDefault(r => r.Cell("B").GetValue<string>() == label);

            if (row == null)
            {
                return Ok(new { Status = true, message = "Label not found in the Excel file.", StatusCode = 404 });
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
                return StatusCode(500, new { Status = true, message = "Error decrypting the code: " + ex.Message, StatusCode = 500 });
            }
            var selectedgiftcard = db.giftCards.Where(x => x.label == label).FirstOrDefault();

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
                , AddDate = selectedgiftcard.AddDate,
                ExpDate = selectedgiftcard.ExpDate
            };

            return Ok(new
            {
                giftCard,
                Status = true,


            });

        }

        [HttpGet("/GetAllGiftCards")]
        public IActionResult GetAllGiftCards()
        {
            var allgiftcards = db.giftCards.OrderByDescending(u => u.Id).ToList();

            return Ok(new { Status = true, allgiftcards });
        }
        [HttpGet("minmaxprice")]
        public IActionResult GetMinMaxPrice()
        {
            var minPrice = db.giftCards.Min(g => g.Price);
            var maxPrice = db.giftCards.Max(g => g.Price);

            var result = new
            {
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            return Ok(result);
        }

        [HttpGet("/SearchGift")]
        public IActionResult SearchGift([FromQuery] string? status, [FromQuery] string? country, [FromQuery] string? type, [FromQuery] double minPrice, [FromQuery] double maxPrice)
        {
            // Validate the price range before querying the database
            if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Invalid price range. Ensure minPrice is less than or equal to maxPrice and both are non-negative."
                });
            }

            // Start with the base query
            var query = db.giftCards.AsQueryable();

            // Apply filters if they are provided
            if (!string.IsNullOrEmpty(status))
                query = query.Where(u => u.Status == status);

            if (!string.IsNullOrEmpty(country))
                query = query.Where(u => u.Country == country);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(u => u.type == type);
            if (!string.IsNullOrEmpty("" + maxPrice))
            {
                // minPrice = db.giftCards.Min(g => g.Price);
                maxPrice = db.giftCards.Max(g => g.Price);


            }
            if (string.IsNullOrEmpty("" + minPrice))
            {
                //  minPrice = db.giftCards.Min(g => g.Price);
                minPrice = db.giftCards.Max(g => g.Price);

            }
            query = query.Where(g => g.Price >= minPrice && g.Price <= maxPrice).OrderByDescending(u => u.Id);

            // Apply price range filter

            // Select the required fields and execute the query
            var giftCards = query
                .Select(g => new
                {
                    g.Code,
                    g.Country,
                    g.ExpDate,
                    g.Price,
                    g.Status,
                    g.label,
                    g.type
                })
                .ToList();

            // Return the filtered gift cards
            return Ok(new { Status = true, giftCards });
        }
        [HttpPost("/DeleteGiftCard")]
        public IActionResult DeleteGiftCard([FromQuery] string label)
        {
            var Gc = db.giftCards.FirstOrDefault(u => u.label == label);
            if (Gc == null)
                return BadRequest(new { Status = false, message = "گیفت کارت پیدا نشد", Gc });

            if (DeleteRow(label))
            {
                db.giftCards.Remove(Gc);
                db.SaveChanges();
                // return Ok(new { Status = true, newlog, message = "امتیازات این کاربر اضافه شد" });
                return Ok(new { Status = true, message = "گیفت کارت با موفقیت حذف شد" });




            }
            return BadRequest(new { Status = false, message = "خطا در حذف گیفت کارت" });
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
                return BadRequest(new { Status = false, message = "گیفت کارتی با این لیبل یافت نشد", giftcard });
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
            giftcard.AddDate = model.AddDate;
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
            var soldgiftcards = db.giftCards.Where(u => u.Status == "فروخته شده").ToList().Count();

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

            return Ok(new { Status = true, tickets });
        }
        [HttpGet("/ShowUserTicketChats")]
        public IActionResult ShowUserTicketChats([FromQuery] int id)
        {
            // Retrieve chats with only the required fields
            var chats = db.ticketChats
                .Where(t => t.TicketId == id)
                .Select(t => new
                {
                    Message = t.message,
                    Image = t.DocumentPath,
                    Sender = t.Sender,
                    SendDate = t.SendDate
                })
                .ToList();

            // Retrieve the ticket status
            var ticketStatus1 = db.tickets
                .Where(t => t.Id == id)
                .Select(t => t.Status)
                .FirstOrDefault(); // Use FirstOrDefault to get a single value
            var ticketStatus = true;
            if (ticketStatus1 == "بسته شده")
            {
                ticketStatus = false;
            }
            // Retrieve the user associated with the ticket
            var user = db.tickets
                .Where(t => t.Id == id)
                .Select(t => t.user) // Assuming `user` is a navigation property in the `Tickets` model
                .FirstOrDefault(); // Use FirstOrDefault to get a single user object

            // Return the response in the desired format
            return Ok(new
            {
                Status = true,
                User = user,
                TicketStatus = ticketStatus,
                Chats = chats
            });
        }
        [HttpGet("/GetImage")]
        public IActionResult GetImage([FromQuery] int id)
        {
            var user = db.tickets.FirstOrDefault(t => t.Id == id);
            if (user == null)
            {
                return Ok(new { Status = true, user, message = "چنین تیکتی موجود نمیباشد", StatusCode = 200 });
            }

            string filePath = db.tickets.FirstOrDefault(x => x.Id == id).DocumentPath;
            if (filePath == null)
            {
                return Ok(new { Status = true, filePath, message = "فایلی موجود نمیباشد", StatusCode = 200 });
            }



            //string contentType = GetContentType(filePath);
            //var fileStream = System.IO.File.OpenRead(filePath);
            return Ok(new { File = filePath });
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

            return Ok(new { Status = true, tickets });
        }

        [HttpGet("/GetAllTicketsByImportance")]
        public IActionResult GetAllTicketsByImportance()
        {
            var tickets = db.tickets.OrderByDescending(t => t.Importance).ToList();

            return Ok(new { Status = true, tickets });
        }

        [HttpGet("/GetAllTicketsByStatus")]
        public IActionResult GetAllTicketsByStatus([FromQuery] string status)
        {
            var tickets = db.tickets.Where(t => t.Status == status).ToList();

            return Ok(new { Status = true, tickets });
        }

        [HttpGet("/GetAllTicketsByTile")]
        public IActionResult GetAllTicketsByTile([FromQuery] string? status)
        {


            if (string.IsNullOrEmpty(status))
            {
                var all = db.tickets.ToList();
                return Ok(new { Status = true, all });

            }

            var tickets = db.tickets.Where(t => t.Title.Contains(status)).ToList();

            return Ok(new { Status = true, tickets });
        }
        [HttpPost("/SendResponeToTickets")]
        public IActionResult SendResponeToTickets([FromQuery] int id, ViewModelResponseToTickets model)
        {
            string filePath = @"wwwroot\Documents";

            Services.Documents documentsService = new Services.Documents();
            string documentPath = documentsService.UploadFile(model.Document, filePath);

            var chat = db.tickets.FirstOrDefault(u => u.Id == id);
            if (chat.Status != "بسته شده")
            {
                if (chat == null)
                {
                    return BadRequest(new { Status = false, chat, message = "تیکتی یافت نشد", StatusCode = 200 });
                }
                TicketChats newRes = new TicketChats();

                newRes.Sender = 1;
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
                return Ok(new { Status = true, message = "تیکت با موفقیت پاسخ داده شد", newRes });




            }
            else
            {
                return Ok(new { Status = false, message = "این چت بسته شده" });
            }

        }

        [HttpPost("/CloseTicket")]
        public IActionResult CloseTicket([FromQuery] int id)
        {
            var ticket = db.tickets.FirstOrDefault(u => u.Id == id);
            if (ticket == null)
            {
                return BadRequest(new { Status = false, ticket, message = "تیکتی یافت نشد", StatusCode = 200 });
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
            if (stars > 0)
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
                return BadRequest(new { Status = false, message = "ستاره نمیتواند کمتر از صفر باشد" });


            }


        }
        [HttpPost("/GetMinStars")]
        public IActionResult GetMinStars()
        {

            var lastRow = db.telegramStars
.OrderByDescending(t => t.Id) // Sort by Id in descending order
.FirstOrDefault(); // Get the first row (which has the highest Id)
            var minStars = lastRow.MinStars;

            return Ok(new { minStars, Status = true });


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
                return BadRequest(new { Status = false, message = "ستاره نمیتواند کمتر از صفر باشد" });
            }
        }

        [HttpPost("/GetStarsPerDollar")]
        public IActionResult GetStarsPerDollar()
        {

            var lastRow = db.telegramStars
.OrderByDescending(t => t.Id) // Sort by Id in descending order
.FirstOrDefault(); // Get the first row (which has the highest Id)
            var minStars = lastRow.StarsPerADollar;

            return Ok(new { minStars, Status = true });


        }
        [HttpGet("/GetAllUsersRequests")]
        public IActionResult GetAllUsersRequests()
        {
            // گروه‌بندی درخواست‌ها بر اساس UserId و انتخاب آخرین درخواست هر گروه
            var uniqueRequests = db.userStarsLogs
                .Where(x => x.Status == "Waiting") // فقط درخواست‌های با وضعیت "Waiting"
                .GroupBy(x => x.UserId) // گروه‌بندی بر اساس UserId
                .Select(g => g.OrderByDescending(x => x.LogDate).First()) // انتخاب آخرین درخواست هر گروه
                .ToList();

            return Ok(new { Status = true, requests = uniqueRequests });
        }


        [HttpPost("/RejectRequest")]
        public IActionResult RejectRequest([FromQuery] int reqid)
        {

            var UserRequest = db.userStarsLogs.Where(x => x.Id == reqid && x.Status == "Waiting").FirstOrDefault();
            if (UserRequest == null)
            {
                return Ok(new { Status = false, message = "درخاستی از طرف این کاربر یافت نشد" });
            }
            else
            {



                UserRequest.Status = "rejected";
                db.SaveChanges();
                return Ok(new { Status = true, UserRequest });





            }

        }

        [HttpPost("/removeStars")]
        public IActionResult removeStars([FromQuery] int reqid)
        {

            var UserRequest = db.userStarsLogs.Where(x => x.Id == reqid && x.Status == "Waiting").FirstOrDefault();
            if (UserRequest == null)
            {
                return Ok(new { Status = false, message = "درخاستی از طرف این کاربر یافت نشد" });
            }
            else
            {
                var lastRow = db.telegramStars
      .OrderByDescending(t => t.Id) // Sort by Id in descending order
    .FirstOrDefault(); // Get the first row (which has the highest Id)
                var minStars = lastRow.MinStars;


                var user = db.users.FirstOrDefault(x => x.Id == UserRequest.UserId);
                user.Stars = 0;
                UserRequest.Status = "success";
                db.SaveChanges();
                return Ok(new { Status = true, user });





            }

        }
        [HttpPost("/DeleteStars")]
        public IActionResult DeleteStars([FromQuery] int id, [FromQuery] int stars)
        {



            var user = db.users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                user.Stars = user.Stars - stars;

                db.SaveChanges();

                var newlog = new UserStarsLog();


                newlog.Star = stars;
                newlog.Type = "0";
                newlog.Status = "success";
                newlog.LogDate = DateTime.Now;
                newlog.UserId = id;





                db.userStarsLogs.Add(newlog);

                db.SaveChanges();
                return Ok(new { Status = true, user, message = "امتیازات این کاربر کم شد شد" });


            }



            else
            {
                return BadRequest(new { Status = false, message = "کاربر یافت نشد" });

            }

        }
        [HttpPost("/AddStars")]
        public IActionResult AddStars([FromQuery] int id, [FromQuery] int addstars)
        {



            var user = db.users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                user.Stars = user.Stars + addstars;
                db.SaveChanges();

                var newlog = new UserStarsLog();


                newlog.Star = addstars;
                newlog.Type = "1";
                newlog.Status = "success";
                newlog.LogDate = DateTime.Now;
                newlog.UserId = id;





                db.userStarsLogs.Add(newlog);

                db.SaveChanges();
                return Ok(new { Status = true, user, message = "امتیازات این کاربر اضافه شد" });


            }



            else
            {
                return BadRequest(new { Status = false, message = "کاربر یافت نشد" });

            }

        }











        [HttpGet("/GetAllRequestsAdmin")]
        public IActionResult GetAllRequests()
        {
            var reqs = db.userStarsLogs.Where(x => x.Status == "Waiting").OrderByDescending(u => u.Id).ToList();
            return Ok(new { reqs, Status = true });


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
                  .Include(f => f.User) // Include the User navigation property
                                        // Filter factors with Status == true
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
                      factorPrice = f.GiftCard.Price,
                      // Combine FirstName and LastName from the User navigation property
                      Username = f.User != null ? $"{f.User.FirstName} {f.User.LastName}" : "Unknown User",
                      // Price from the GiftCards table (handle null GiftCard)
                      GiftCardPrice = f.GiftCard != null ? f.GiftCard.Price : (double?)null
                  })
                  .OrderByDescending(u => u.FactorId)
                  .ToList();

            return Ok(new { factors, Status = true }); // Return the filtered list of factors
                                                       // Return the list of factors
        }
        [HttpGet("/GetAllFactorsFail")]
        public IActionResult GetAllFactorsFail()
        {
            var factors = db.factors
                    .Include(f => f.GiftCard) // Include the GiftCard navigation property
                    .Include(f => f.User) // Include the User navigation property
                    .Where(f => f.Status == "success") // Filter factors with Status == true
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
                        factorPrice = f.GiftCard.Price,
                        // Combine FirstName and LastName from the User navigation property
                        Username = f.User != null ? $"{f.User.FirstName} {f.User.LastName}" : "Unknown User",
                        // Price from the GiftCards table (handle null GiftCard)
                        GiftCardPrice = f.GiftCard != null ? f.GiftCard.Price : (double?)null
                    })
                    .ToList();

            return Ok(new { factors, Status = true }); // Return the filtered list of factors
                                                       // Return the filtered list of factors
        }




        [HttpGet("/GetAllFactorsTrue")]
        public IActionResult GetAllFactorsTrue()
        {
            var factors = db.factors
                .Include(f => f.GiftCard) // Include the GiftCard navigation property
                .Include(f => f.User) // Include the User navigation property
                .Where(f => f.Status == "success") // Filter factors with Status == true
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
                    factorPrice = f.GiftCard.Price,
                    // Combine FirstName and LastName from the User navigation property
                    Username = f.User != null ? $"{f.User.FirstName} {f.User.LastName}" : "Unknown User",
                    // Price from the GiftCards table (handle null GiftCard)
                    GiftCardPrice = f.GiftCard != null ? f.GiftCard.Price : (double?)null
                })
                .ToList();

            return Ok(new { factors, Status = true }); // Return the filtered list of factors
        }



        [HttpGet("/SearchInFailFactorsByName")]
        public IActionResult SearchInFailFactorsByName(
       [FromQuery] string? name,
       [FromQuery] string? status) // Make status optional
        {
            // Start with the base query
            var query = db.factors
                .Include(f => f.GiftCard) // Include the GiftCard navigation property
                .Include(f => f.User) // Include the User navigation property
                .AsQueryable();

            // Apply name filter if name is provided
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(f => f.UserName.Contains(name));
            }

            // Apply status filter if status is provided
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(f => f.Status == status);
            }

            // Select the required fields and execute the query
            var factors = query
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
                    FactorPrice = f.GiftCard.Price,
                    // Combine FirstName and LastName from the User navigation property
                    Username = f.User != null ? $"{f.User.FirstName} {f.User.LastName}" : "Unknown User",
                    // Price from the GiftCards table (handle null GiftCard)
                    GiftCardPrice = f.GiftCard != null ? f.GiftCard.Price : (double?)null
                })
                .ToList();

            // Check if any factors match the criteria
            if (factors == null || !factors.Any())
            {
                return Ok(new
                {
                    Status = true,
                    Message = "No factors found matching the specified criteria.",
                    factors = new List<object>() // Return an empty list
                });
            }

            // Return the filtered factors
            return Ok(new { factors, Status = true });
        }


        [HttpGet("/GetSummary")]
        public IActionResult GetSummary()
        {
            DateTime now = DateTime.Now;
            DateTime startOfToday = now.Date; // Start of the current day (midnight)
            DateTime startOfLastWeek = now.AddDays(-7);
            DateTime startOfLastMonth = now.AddMonths(-1);
            DateTime startOfLastMonthRegistration = now.AddMonths(-1).Date; // Start of last month

            // Calculate sums
            double sumToday = db.factors
                .Where(f => f.FactorDate >= startOfToday && f.FactorDate <= now)
                .Sum(f => f.GiftCard.Price);

            double sumLastWeek = db.factors
                .Where(f => f.FactorDate >= startOfLastWeek && f.FactorDate <= now)
                .Sum(f => f.GiftCard.Price);

            double sumLastMonth = db.factors
                .Where(f => f.FactorDate >= startOfLastMonth && f.FactorDate <= now)
                .Sum(f => f.GiftCard.Price);

            // Count users with no email address
            int usersWithNoEmailCount = db.users
                .Count(u => string.IsNullOrEmpty(u.Email)); // Assuming Email is a string column

            // Count users with an email address
            int usersWithEmailCount = db.users
                .Count(u => !string.IsNullOrEmpty(u.Email)); // Users with a non-empty email

            // Count users who registered last month
            int usersRegisteredLastMonthCount = db.users
                .Where(u => u.RegisterDate >= startOfLastMonthRegistration && u.RegisterDate < startOfLastMonthRegistration.AddMonths(1))
                .Count();

            // Sum of FactorPrice for factors with Status == true
            int CountOfsuccessFactors = db.factors
                .Where(f => f.Status == "success")
                .Count(); // Use 0 if FactorPrice is null
            int CountOfUnsuccessFactors = db.factors
               .Where(f => f.Status != "success")
               .Count(); // Use 0 if FactorPrice is null

            // Get the country with the highest sales
            var countrySales = db.factors
                .GroupBy(f => f.GiftCard.Country)
                .Select(g => new
                {
                    Country = g.Key,
                    TotalSales = g.Sum(f => f.GiftCard.Price)
                })
                .OrderByDescending(g => g.TotalSales)
                .FirstOrDefault();

            // Get the type of gift card with the highest sales
            var typeSales = db.factors
                .GroupBy(f => f.GiftCard.type)
                .Select(g => new
                {
                    Type = g.Key,
                    TotalSales = g.Sum(f => f.GiftCard.Price)
                })
                .OrderByDescending(g => g.TotalSales)
                .FirstOrDefault();

            return Ok(new
            {
                Status = true,
                SumToday = sumToday,
                SumLastWeek = sumLastWeek,
                SumLastMonth = sumLastMonth,
                UsersWithNoEmailCount = usersWithNoEmailCount,
                UsersWithEmailCount = usersWithEmailCount,
                UsersRegisteredLastMonthCount = usersRegisteredLastMonthCount,
                CountFactorsWithTrueStatus = CountOfsuccessFactors, // Updated to sum instead of count
                CountFactorsWithFalseStatus = CountOfUnsuccessFactors, // Updated to sum instead of count
                TopSellingCountry = countrySales?.Country ?? "No data",
                TopSellingCountrySales = countrySales?.TotalSales ?? 0,
                TopSellingType = typeSales?.Type ?? "No data",
                TopSellingTypeSales = typeSales?.TotalSales ?? 0
            });
        }


        [HttpGet("/GetCharts")]
        public IActionResult GetCharts()
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

            // Initialize a dictionary with all 12 months and default sales value of 0.0 (double)
            var allMonths = Enumerable.Range(1, 12).ToDictionary(
                month => month,
                month => new { name = new DateTime(currentYear, month, 1).ToString("MMMM"), sales = 0.0 } // Use 0.0 for double
            );

            // Update the dictionary with data from the query
            foreach (var income in monthlyIncome)
            {
                allMonths[income.Month] = new
                {
                    name = new DateTime(currentYear, income.Month, 1).ToString("MMMM"),
                    sales = income.Sales // This is already a double
                };
            }

            // Convert the dictionary to a list of monthly income results
            var monthlyIncomeResult = allMonths.Values.OrderBy(m => DateTime.ParseExact(m.name, "MMMM", CultureInfo.InvariantCulture).Month).ToList();

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
        [HttpGet("/GetActivitylogs")]
        public IActionResult GetActivitylogs()
        {

            var logs = db.activityLogs.OrderByDescending(u => u.Id).ToList();
            return Ok(new { Status = true, Logs = logs });

        }

        [HttpPost("/AddActivitylogs")]
        public IActionResult AddActivitylogs([FromBody] AddActivityLogViewModel model)
        {
            var newlog = new ActivityLog
            {
                UserId = model.UserId,
                Description = model.Description,
                LogDate = DateTime.Now,
                UserRole = db.users.Where(x => x.Id == model.UserId).Select(x => x.Role).FirstOrDefault(),
                Name = db.users.Where(x => x.Id == model.UserId).Select(x => x.LastName).FirstOrDefault() + db.users.Where(x => x.Id == model.UserId).Select(x => x.LastName).FirstOrDefault()
            };
            db.activityLogs.Add(newlog);
            db.SaveChanges();

            return Ok(new { Status = true, newlog });


        }


        [HttpGet("/GetActivitylogsEnterExit")]
        public IActionResult GetActivitylogsEnterExit()
        {

            var logs = db.activityLogs.Where(x => x.Description == "ورود" || x.Description == "خروج").ToList();
            return Ok(new { Status = true, Logs = logs });

        }


        [HttpPost("/AddNotifications")]
        public IActionResult AddNotifications(AddNotificationViewModel model)

        {

            var path = "";
            Services.Documents documentsService = new Services.Documents();
            string filePath = @"wwwroot\Documents";

            if (!string.IsNullOrEmpty(documentsService.UploadFile(model.Document, filePath)))
            {
                path = documentsService.UploadFile(model.Document, filePath);
            }
            else
            {
                path = null;
            }




            var Username = "ALL";


            var SHOW = "";
            if (string.IsNullOrEmpty(model.ShowTo))
            {
                SHOW = "0";
            }
            else {
                SHOW = model.ShowTo;


                Username = db.users.Where(x => x.Id == int.Parse(model.ShowTo)).Select(x => x.Username).FirstOrDefault();

            }
            var newNotif = new Notifications
            {
                Title = model.Title,
                Description = model.Description,
                DateOfNotification = DateTime.Now,
                Status = 0,
                showTo = SHOW,
                Type = model.Type,
                Username = Username,
                DocumentPath = path
            };

            db.notifications.Add(newNotif);
            db.SaveChanges();


            return Ok(new { Status = true, newNotif });

        }

        [HttpGet("/GetNotificationsAdmin")]
        public IActionResult GetNotificationsAdmin()
        {
            var AllNotifs = db.notifications.OrderByDescending(x => x.Id).ToList();

            return Ok(new { Status = true, AllNotifs });


        }


        [HttpGet("/GetWalletLogOfUser")]
        public IActionResult GetWalletLogOfUser(int userid)
        {
            var AllLogs = db.walletLogs.OrderByDescending(x => x.Id).Where(x => x.UserId == userid).ToList();

            return Ok(new { Status = true, AllLogs });


        }


        [HttpGet("/GetFactorsOfUser")]
        public IActionResult GetFactorsOfUser(int userid)
        {
            var AllLogs = db.factors.OrderByDescending(x => x.Id).Where(x => x.UserId == userid).ToList();

            return Ok(new { Status = true, AllLogs });


        }

        [HttpGet("/GetStarLogsOfUser")]
        public IActionResult GetStarLogsOfUser(int userid)
        {
            var AllLogs = db.userStarsLogs.OrderByDescending(x => x.Id).Where(x => x.UserId == userid).ToList();

            return Ok(new { Status = true, AllLogs });


        }

        [HttpGet("/GetTicketsOfUser")]
        public IActionResult GetTicketsOfUser(int userid)
        {
            var AllLogs = db.tickets.OrderByDescending(x => x.Id).Where(x => x.UserId == userid).ToList();

            return Ok(new { Status = true, AllLogs });


        }
        [HttpGet("/GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            var AllRoles = db.roles.OrderByDescending(x => x.Id).ToList();

            return Ok(new { Status = true, AllRoles });


        }

        [HttpGet("/GetAllSaths")]
        public IActionResult GetAllSaths()
        {
            // ایجاد لیست مقادیر "کم"، "متوسط" و "زیاد"
            var saths = new List<string> { "کم", "متوسط", "زیاد" };

            return Ok(new { Status = true, Saths = saths });
        }
        [HttpPost("/AddRole")]
        public IActionResult AddRole(string rolename, string sath)
        {
            var role = new Roles
            {
                RoleName = rolename,
                sath = sath
            };

            db.roles.Add(role);
            db.SaveChanges();

            return Ok(new { Status = true, role });

        }
        [HttpPost("/ChangeRole")]
        public IActionResult ChangeRole(string phone, int roleid)
        {
            var user = db.users.FirstOrDefault(x => x.Phone == phone);
            if (user == null)
            {
                return BadRequest(new { Status = false, message = "user not found" });
            }
            else
            {
                var role = db.roles.FirstOrDefault(x => x.Id == roleid);

                var token = GenerateJwtToken(user);


                user.Role = role.RoleName;


                db.SaveChanges();

                return Ok(new { Status = true, user,token });

            }

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


        [HttpGet("/SearchNotifs")]
        public IActionResult SearchNotifs([FromQuery] string? onvan)
        {
            // Start with the base query
            var query = db.notifications.AsQueryable();

            // Apply title filter if onvan is provided
            if (!string.IsNullOrEmpty(onvan))
            {
                query = query.Where(x => x.Title .Contains( onvan));
            }

            // Execute the query and get the results
            var notifs = query.ToList();

            // Return the results
            return Ok(new { notifs, Status = true });
        }

        [HttpGet("/SearchContactUs")]
        public IActionResult SearchContactUs([FromQuery] string? onvan)
        {
            // Start with the base query
            var query = db.contact_Us.AsQueryable();

            // Apply text filter if onvan is provided
            if (!string.IsNullOrEmpty(onvan))
            {
                query = query.Where(x => x.Text .Contains( onvan));
            }

            // Execute the query and get the results
            var contactus = query.ToList();

            // Return the results
            return Ok(new { contactus, Status = true });
        }



    

    //    [HttpGet("SearchFactors")]
    //    public async Task<IActionResult> SearchFactors(
    //[FromQuery] DateTime? startDate = null,
    //[FromQuery] DateTime? endDate = null,
    //[FromQuery] string? name = null)
    //    {
    //        // Validate date range if both startDate and endDate are provided
    //        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
    //        {
    //            return BadRequest("Start date cannot be greater than end date.");
    //        }

    //        // Base query
    //        var query = db.factors
    //            .Include(f => f.User) // Include related User data
    //            .Include(f => f.GiftCard) // Include related GiftCard data
    //            .AsQueryable();

    //        // Apply date filter if startDate and endDate are provided
    //        if (startDate.HasValue && endDate.HasValue)
    //        {
    //            query = query.Where(f => f.FactorDate >= startDate && f.FactorDate <= endDate);
    //        }

    //        // Apply name filter if name is provided
    //        if (!string.IsNullOrEmpty(name))
    //        {
    //            query = query.Where(f => f.UserName.Contains(name));
    //        }

    //        // Project the results
    //        var factors = await query
    //            .Select(f => new
    //            {
    //                // Fields from the Factors table
    //                FactorId = f.Id,
    //                UserId = f.UserId,
    //                FactorDate = f.FactorDate,
    //                GiftId = f.GiftId,
    //                Status = f.Status,
    //                Type = f.Type,
    //                TransActionNumber = f.TransActionNumber,
    //                FactorPrice = f.GiftCard.Price,
    //                // Combine FirstName and LastName from the User navigation property
    //                Username = f.User != null ? $"{f.User.FirstName} {f.User.LastName}" : "Unknown User",
    //                // Price from the GiftCards table (handle null GiftCard)
    //                GiftCardPrice = f.GiftCard != null ? f.GiftCard.Price : (double?)null
    //            })
    //            .ToListAsync();

    //        // Check if any factors match the criteria
    //        if (factors == null || !factors.Any())
    //        {
    //            return Ok(new { Message = "No factors found matching the specified criteria.", Status = true });
    //        }

    //        return Ok(new { factors, Status = true });
    //    }


        [HttpGet("/SearchFactorsByNameAndDate")]
        public IActionResult SearchFactorsByNameAndDate(
       [FromQuery] string? name, // Optional: Filter by user name
       [FromQuery] DateTime? startDate , // Optional: Filter by start date
       [FromQuery] DateTime? endDate ) // Optional: Filter by end date
        {
            // Validate date range if both startDate and endDate are provided
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Start date cannot be greater than end date."
                });
            }

            // Start with the base query
            var query = db.factors
                .Include(f => f.User) // Include related User data
                .Include(f => f.GiftCard) // Include related GiftCard data
                .AsQueryable();

            // Apply name filter if name is provided
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(f => f.UserName.Contains(name)); // Assuming 'UserName' is the field for the user's name
            }

        
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(f => f.FactorDate >= startDate && f.FactorDate <= endDate); // Assuming 'FactorDate' is the field for the factor date
            }

            // Select the required fields and execute the query
            var factors = query
                .OrderByDescending(f => f.Id) // Order by Id in descending order
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
                    FactorPrice = f.GiftCard.Price,
                    // Combine FirstName and LastName from the User navigation property
                    Username = f.User != null ? $"{f.User.Username} " : "Unknown User",
                    // Price from the GiftCards table (handle null GiftCard)
                    GiftCardPrice = f.GiftCard != null ? f.GiftCard.Price : (double?)null
                })
                .ToList();

            // Check if any factors match the criteria
            if (factors == null || !factors.Any())
            {
                return Ok(new
                {
                    Status = true,
                    Message = "No factors found matching the specified criteria.",
                    factors = new List<object>() // Return an empty list
                });
            }

            // Return the filtered factors
            return Ok(new { Status = true, factors });
        }
        [HttpPost("/DeleteContactUs")]
        public IActionResult DeleteContactUs(int id)
        {
            var contactUs = db.contact_Us.FirstOrDefault(x=>x.Id==id);
            if (contactUs == null)
            {
                return NotFound();
            }

            db.contact_Us.Remove(contactUs);
             db.SaveChanges();

            return Ok(new { Status = true, message = " درخاست  با موفقیت حذف شد" });

        }
        [HttpPost("DeleteAllContactUs")]
        public IActionResult DeleteAllContactUs()
        {
            var all = db.contact_Us.ToList();
            if (all.Any())
            {
                db.contact_Us.ExecuteDelete();
                return Ok(new { Status = true, message = "تمام درخاست ها با موفقیت حذف شد" });

            }
            else
            {
                return BadRequest(new { Status = false, message = "درخاستی موجود نمیباشد" });

            }
           
        }


    }
}