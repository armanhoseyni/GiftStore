using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GiftStore.ViewModels.Payment;
using Microsoft.AspNetCore.Mvc;

namespace GiftStore.Services.Payment
{
     public class ZarinPalService : IZarinPalService
{
    private readonly ILogger<ZarinPalService> _logger;
    private readonly IConfiguration _configuration;

    public ZarinPalService(ILogger<ZarinPalService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<PaymentResponse> RequestPaymentAsync(PaymentRequest paymentRequest)
    {
        var merchantId = _configuration["Zarinpal:MerchantId"];
        var callbackUrl = _configuration["Zarinpal:CallbackUrl"];
        var sandbox = _configuration.GetValue<bool>("Zarinpal:Sandbox");

        var paymentData = new
        {
            merchant_id = merchantId,
            amount = paymentRequest.Amount,
            description = paymentRequest.Description,
            callback_url = callbackUrl,
            metadata = new { mobile = paymentRequest.Mobile, email = paymentRequest.Email }
        };

        using (var client = new HttpClient())
        {
            var url = sandbox ? "https://sandbox.zarinpal.com/pg/v4/payment/request.json" : "https://payment.zarinpal.com/pg/v4/payment/request.json";
            var response = await client.PostAsJsonAsync(url, paymentData);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(result);

                if (jsonResponse.TryGetProperty("data", out JsonElement dataElement))
                {
                    var authority = dataElement.GetProperty("authority").GetString();
                    return new PaymentResponse
                    {
                        Status = 100,
                        Authority = authority,
                        PaymentUrl = $"https://sandbox.zarinpal.com/pg/StartPay/{authority}"
                    };
                }
            }
        }

        throw new Exception("Payment request failed.");
    }

    public async Task<IActionResult> VerifyPaymentAsync(string authority, int amount)
    {
        var merchantId = _configuration["Zarinpal:MerchantId"];
        var paymentData = new
        {
            merchant_id = merchantId,
            amount = amount,
            authority = authority
        };

        using (var client = new HttpClient())
        {
            var url = "https://sandbox.zarinpal.com/pg/v4/payment/verify.json";
            var response = await client.PostAsJsonAsync(url, paymentData);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(result);

                _logger.LogInformation("Zarinpal Response: " + result);

                if (jsonResponse.TryGetProperty("data", out JsonElement dataElement))
                {
                    if (dataElement.TryGetProperty("code", out JsonElement statusElement))
                    {
                        if (statusElement.GetInt32() == 100)
                        {
                            if (dataElement.TryGetProperty("ref_id", out JsonElement refIdElement))
                            {
                                return new JsonResult(new { Message = "Payment successful.", RefId = refIdElement.GetInt32() });
                            }
                            else
                            {
                                return new BadRequestObjectResult("RefId not found in the response.");
                            }
                        }
                        else
                        {
                            return new BadRequestObjectResult($"Payment verification failed. Status code: {statusElement.GetInt32()}");
                        }
                    }
                    else
                    {
                        return new BadRequestObjectResult("Code not found in the response.");
                    }
                }
                else
                {
                    return new BadRequestObjectResult("Data not found in the response.");
                }
            }
            else
            {
                return new BadRequestObjectResult("Failed to verify payment.");
            }
        }
    }
}
}