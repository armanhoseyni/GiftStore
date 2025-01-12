using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiftStore.ViewModels.Payment
{
    public class PaymentRequest
    {
    public int Amount { get; set; }
    public string Description { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    }
}