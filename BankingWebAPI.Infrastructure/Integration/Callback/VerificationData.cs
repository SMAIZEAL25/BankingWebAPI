using Newtonsoft.Json;

namespace BankingWebAPI.Infrastructure.Integration.Callback
{
    public class VerificationData
    {
        [JsonProperty("status")]
        public string Status { get; set; } // "success", "failed", "abandoned"

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("AuthorizationUrl")]
        public string AuthorizationUrl { get; set; }

        [JsonProperty("AccessCode")]
        public string AccessCode { get; set; }


    }
}
