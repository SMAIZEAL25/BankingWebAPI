using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingWebAPI.Infrastructure.Integration.Response
{
    public class PaymentRequest
    {

        public decimal Amount { get; set; }
        public string Email { get; set; }
        public string Reference { get; set; }
        public string CallbackUrl { get; set; }
        public string Currency { get; set; } = "NGN";
        public object Metadata { get; set; }
    }
}


