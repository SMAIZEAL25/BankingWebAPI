
using BankingWebAPI.Infrastructure.Integration.Callback;
using BankingWebAPI.Infrastructure.Integration.Response;
using BankingWebAPI.Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BankingWebAPI.Infrastructure.Integration
{
    //public class PaystackService : IPaymentGateway
    //{
    //    private readonly HttpClient _httpClient;
    //    private readonly string _secretKey;
    //    private readonly ILogger<PaystackService> _logger;

    //    public PaystackService(
    //        HttpClient httpClient,
    //        IConfiguration config,
    //        ILogger<PaystackService> logger)
    //    {
    //        _httpClient = httpClient;
    //        _secretKey = config["Paystack:SecretKey"];
    //        _logger = logger;
    //    }

    //    public async Task<PaystackVerificationResponse> InitializeTransaction(PaymentRequest request)
    //    {
    //        // Mocked response for testing
    //        if (_secretKey == "test_key")
    //        {
    //            return new PaystackVerificationResponse
    //            {
    //                Status = true,
    //                Message = "Authorization URL created",
    //                Data = new TransactionData
    //                {
    //                    AuthorizationUrl = "https://paystack.mock/pay/" + request.Reference,
    //                    AccessCode = "mock_access_code",
    //                    Reference = request.Reference
    //                }
    //            };
    //        }

    //        // Convert Naira → Kobo before sending to Paystack
    //        var paystackPayload = new
    //        {
    //            amount = (int)(request.Amount * 100), // Paystack expects integer in kobo
    //            email = request.Email,
    //            reference = request.Reference,
    //            callback_url = request.CallbackUrl,
    //            currency = request.Currency,
    //            metadata = request.Metadata
    //        };

    //        var content = new StringContent(
    //            JsonConvert.SerializeObject(paystackPayload),
    //            Encoding.UTF8,
    //            "application/json");

    //        _httpClient.DefaultRequestHeaders.Authorization =
    //            new AuthenticationHeaderValue("Bearer", _secretKey);

    //        var response = await _httpClient.PostAsync(
    //            "https://api.paystack.co/transaction/initialize",
    //            content);

    //        return await response.Content.ReadFromJsonAsync<PaystackTransactionResponse>();
    //    }
    //}

    public class PaystackService : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly ILogger<PaystackService> _logger;

        public PaystackService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<PaystackService> logger)
        {
            _httpClient = httpClient;
            _secretKey = config["Paystack:SecretKey"];
            _logger = logger;
        }

        public async Task<PaystackVerificationResponse> InitializeTransaction(PaymentRequest request)
        {
            // Mocked response for testing
            if (_secretKey == "test_key")
            {
                return new PaystackVerificationResponse
                {
                    Status = true,
                    Message = "Authorization URL created",
                    Data = new VerificationData   // ✅ changed from TransactionData → VerificationData
                    {
                        AuthorizationUrl = "https://paystack.mock/pay/" + request.Reference,
                        AccessCode = "mock_access_code",
                        Reference = request.Reference
                    }
                };
            }

            // Convert Naira → Kobo before sending to Paystack
            var paystackPayload = new
            {
                amount = (int)(request.Amount * 100), // Paystack expects integer in kobo
                email = request.Email,
                reference = request.Reference,
                callback_url = request.CallbackUrl,
                currency = request.Currency,
                metadata = request.Metadata
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(paystackPayload),
                Encoding.UTF8,
                "application/json");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _secretKey);

            var response = await _httpClient.PostAsync(
                "https://api.paystack.co/transaction/initialize",
                content);

            var rawResponse = await response.Content.ReadFromJsonAsync<PaystackTransactionResponse>();

            // ✅ map PaystackTransactionResponse → PaystackVerificationResponse
            return new PaystackVerificationResponse
            {
                Status = rawResponse.Status,
                Message = rawResponse.Message,
                Data = new VerificationData
                {
                    AuthorizationUrl = rawResponse.Data.AuthorizationUrl,
                    AccessCode = rawResponse.Data.AccessCode,
                    Reference = rawResponse.Data.Reference
                }
            };
        }
    }
}


