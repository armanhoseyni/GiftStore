
using GiftStore.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GiftStore.Services
{
    public class SMSirService
    {
        private readonly SMSirConfig _config;
        private readonly HttpClient _httpClient;

        public SMSirService(IOptions<SMSirConfig> config, HttpClient httpClient)
        {
            _config = config.Value;
            _httpClient = httpClient;
        }

        public async Task SendSMSAsync(string mobileNumber, string message)
        {
            var url = "https://api.sms.ir/v1/send";
            var content = new StringContent(
                $"{{\"lineNumber\": \"{_config.LineNumber}\", \"mobile\": \"{mobileNumber}\", \"message\": \"{message}\"}}",
                Encoding.UTF8,
                "application/json"
            );

            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _config.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-SECRET-KEY", _config.SecretKey);

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
