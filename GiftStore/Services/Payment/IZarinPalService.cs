using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GiftStore.ViewModels.Payment;
using Microsoft.AspNetCore.Mvc;

namespace GiftStore.Services.Payment
{
  public interface IZarinPalService
    {
        Task<PaymentResponse> RequestPaymentAsync(PaymentRequest paymentRequest);
    Task<IActionResult> VerifyPaymentAsync(string authority, int amount);
    }
}