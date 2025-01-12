using System.Threading.Tasks;
using GiftStore.Services.Sms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GiftStore.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ISmsService _smsService;
        private readonly ILogger<TestController> _logger;

        public TestController(ISmsService smsService, ILogger<TestController> logger)
        {
            _smsService = smsService;
            _logger = logger;
        }

        [HttpPost("send-sms")]
        public async Task<IActionResult> SendSms([FromBody] SmsRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.Message))
            {
                _logger.LogError("Validation error: Phone number or message is empty.");
                return BadRequest(new { error = "Phone number and message cannot be empty." });
            }

            try
            {
                var result = await _smsService.SendSmsAsync(request.PhoneNumber, request.Message, request.TemplateType);

                if (result)
                {
                    _logger.LogInformation($"SMS successfully sent to {request.PhoneNumber} using template {request.TemplateType}.");
                    return Ok(new { success = true, message = "SMS sent successfully." });
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
    }

    public class SmsRequest
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public string TemplateType { get; set; }
    }
}
