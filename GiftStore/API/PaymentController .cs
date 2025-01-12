using GiftStore.Services.Payment;
using GiftStore.ViewModels.Payment;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GiftStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IZarinPalService _paymentService;


        public PaymentController(ILogger<PaymentController> logger, IConfiguration configuration, IZarinPalService paymentService)
        {
            _logger = logger;
            _configuration = configuration;
            _paymentService = paymentService;

        }

        // اولین متد 

        // [HttpPost("request")]
        // public async Task<IActionResult> RequestPayment([FromBody] PaymentRequest paymentRequest)
        // {
        //     var merchantId = _configuration["Zarinpal:MerchantId"];
        //     var sandbox = _configuration.GetValue<bool>("Zarinpal:Sandbox");
        //     var callbackUrl = _configuration["Zarinpal:CallbackUrl"];

        //     var paymentData = new
        //     {
        //         merchant_id = merchantId,
        //         amount = paymentRequest.Amount,
        //         description = paymentRequest.Description,
        //         callback_url = callbackUrl,
        //         metadata = new { mobile = paymentRequest.Mobile, email = paymentRequest.Email }
        //     };

        //     using (var client = new HttpClient())
        //     {
        //         var url = sandbox ? "https://sandbox.zarinpal.com/pg/v4/payment/request.json" : "https://payment.zarinpal.com/pg/v4/payment/request.json";
        //         var response = await client.PostAsJsonAsync(url, paymentData);
        //         if (response.IsSuccessStatusCode)
        //         {
        //             var result = await response.Content.ReadAsStringAsync();
        //             var jsonResponse = JsonSerializer.Deserialize<JsonElement>(result);

        //             if (jsonResponse.TryGetProperty("data", out JsonElement dataElement))
        //             {
        //                 var authority = dataElement.GetProperty("authority").GetString();
        //                 return Ok(new PaymentResponse
        //                 {
        //                     Status = 100,
        //                     Authority = authority,
        //                     PaymentUrl = $"https://sandbox.zarinpal.com/pg/StartPay/{authority}"
        //                 });
        //             }
        //         }
        //     }

        //     return BadRequest("Payment request failed.");
        // }



        //  خواندن MerchantId و CallbackUrl از appsetting.json           دومین متد

        // [HttpPost("request")]
        // public async Task<IActionResult> RequestPayment([FromBody] PaymentRequest paymentRequest)
        // {
        //     // خواندن MerchantId و CallbackUrl از appsettings.json
        //     var merchantId = _configuration["Zarinpal:MerchantId"];
        //     var callbackUrl = _configuration["Zarinpal:CallbackUrl"];
        //     var sandbox = _configuration.GetValue<bool>("Zarinpal:Sandbox");

        //     var paymentData = new
        //     {
        //         merchant_id = merchantId,
        //         amount = paymentRequest.Amount,
        //         description = paymentRequest.Description,
        //         callback_url = callbackUrl,
        //         metadata = new { mobile = paymentRequest.Mobile, email = paymentRequest.Email }
        //     };

        //     using (var client = new HttpClient())
        //     {
        //         var url = sandbox ? "https://sandbox.zarinpal.com/pg/v4/payment/request.json" : "https://payment.zarinpal.com/pg/v4/payment/request.json";
        //         var response = await client.PostAsJsonAsync(url, paymentData);
        //         if (response.IsSuccessStatusCode)
        //         {
        //             var result = await response.Content.ReadAsStringAsync();
        //             var jsonResponse = JsonSerializer.Deserialize<JsonElement>(result);

        //             if (jsonResponse.TryGetProperty("data", out JsonElement dataElement))
        //             {
        //                 var authority = dataElement.GetProperty("authority").GetString();
        //                 return Ok(new PaymentResponse
        //                 {
        //                     Status = 100,
        //                     Authority = authority,
        //                     PaymentUrl = $"https://sandbox.zarinpal.com/pg/StartPay/{authority}"
        //                 });
        //             }
        //         }
        //     }

        //     return BadRequest("Payment request failed.");
        // }

        // [HttpPost("verify")]
        // public async Task<IActionResult> VerifyPayment([FromQuery] string authority, [FromQuery] int amount)
        // {
        //     var merchantId = _configuration["Zarinpal:MerchantId"];
        //     var paymentData = new
        //     {
        //         merchant_id = merchantId,
        //         amount = amount,
        //         authority = authority
        //     };

        //     using (var client = new HttpClient())
        //     {
        //         var url = "https://sandbox.zarinpal.com/pg/v4/payment/verify.json";
        //         var response = await client.PostAsJsonAsync(url, paymentData);
        //         if (response.IsSuccessStatusCode)
        //         {
        //             var result = await response.Content.ReadAsStringAsync();
        //             var jsonResponse = JsonSerializer.Deserialize<JsonElement>(result);

        //             // چاپ پاسخ زرین‌پال برای بررسی بیشتر
        //             _logger.LogInformation("Zarinpal Response: " + result);

        //             // بررسی وجود بخش "data"
        //             if (jsonResponse.TryGetProperty("data", out JsonElement dataElement))
        //             {
        //                 // بررسی وجود "code" در بخش "data"
        //                 if (dataElement.TryGetProperty("code", out JsonElement statusElement))
        //                 {
        //                     if (statusElement.GetInt32() == 100)
        //                     {
        //                         // بررسی وجود "ref_id" در بخش "data"
        //                         if (dataElement.TryGetProperty("ref_id", out JsonElement refIdElement))
        //                         {
        //                             return Ok(new { Message = "Payment successful.", RefId = refIdElement.GetInt32() });
        //                         }
        //                         else
        //                         {
        //                             return BadRequest("RefId not found in the response.");
        //                         }
        //                     }
        //                     else
        //                     {
        //                         return BadRequest($"Payment verification failed. Status code: {statusElement.GetInt32()}");
        //                     }
        //                 }
        //                 else
        //                 {
        //                     return BadRequest("Code not found in the response.");
        //                 }
        //             }
        //             else
        //             {
        //                 return BadRequest("Data not found in the response.");
        //             }
        //         }
        //         else
        //         {
        //             return BadRequest("Failed to verify payment.");
        //         }
        //     }
        // }


    // صدا زدن سرویس های پرداخت 
    [HttpPost("request")]
    public async Task<IActionResult> RequestPayment([FromBody] PaymentRequest paymentRequest)
    {
        try
        {
            var paymentResponse = await _paymentService.RequestPaymentAsync(paymentRequest);
            return Ok(paymentResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyPayment([FromQuery] string authority, [FromQuery] int amount)
    {
        try
        {
            var result = await _paymentService.VerifyPaymentAsync(authority, amount);
            return result;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    }

}
