
using BankingWebAPI.Infrastructure.Integration.Callback;
using BankingWebAPI.Infrastructure.Integration.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingWebAPI.Infrastructure.Services.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaystackVerificationResponse> InitializeTransaction(PaymentRequest request);
    }
}
