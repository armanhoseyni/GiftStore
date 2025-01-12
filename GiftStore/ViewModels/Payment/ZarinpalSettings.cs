using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiftStore.ViewModels.Payment
{
    public class ZarinpalSettings
    {
         public string MerchantId { get; set; }
    public string CallbackUrl { get; set; }
    public bool Sandbox { get; set; }
    }
}