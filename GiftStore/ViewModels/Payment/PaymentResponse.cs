using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiftStore.ViewModels.Payment
{
    public class PaymentResponse
    {
         public int Status { get; set; }
    public string Authority { get; set; }
    public string PaymentUrl { get; set; }
    }
}