using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GiftStore.Services.Sms
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message, string templateType)
        {
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Phone number and message cannot be empty.");
            }

            try
            {
                using var httpClient = new HttpClient();
                var apiKey = _configuration.GetValue<string>("SmsService:ApiKey");

                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("SMS API key is missing in configuration.");
                }

                httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

                // Retrieve templates
                var templates = _configuration.GetSection("SmsService:Templates").Get<Dictionary<string, int>>();
                if (templates == null || !templates.ContainsKey(templateType))
                {
                    throw new ArgumentException($"Invalid template type: {templateType}");
                }

                var templateId = templates[templateType];

                // Prepare payload
                var payload = new
                {
                    mobile = phoneNumber,
                    templateId,
                    parameters = new[]
                    {
                        new { name = "CONTACTS", value = message }
                    }
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                // Send HTTP request
                var response = await httpClient.PostAsync("https://api.sms.ir/v1/send/verify", content);

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"SMS API Error: {responseContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending SMS: {ex.Message}");
                return false;
            }
        }
    }
}
