using System.Threading.Tasks;

namespace GiftStore.Services.Sms
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string phoneNumber, string message, string templateType);
    }
}
