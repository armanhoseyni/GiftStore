using GiftStore.Data;
using GiftStore.Models;
using GiftStore.Services;
using GiftStore.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GiftStore.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public Db_API db { get; set; }
        private readonly SMSirService _smsirService;

        public AccountController(Db_API db_, SMSirService smsirService)
        {
            db = db_;
            _smsirService = smsirService;

        }

        [HttpPost("send")]
        public async Task<IActionResult> SendSMS([FromBody] SMSRequest request)
        {
            await _smsirService.SendSMSAsync(request.MobileNumber, request.Message);
            return Ok();
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
    }
}
