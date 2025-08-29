using BankingApp.Domain.Enums;
using BankingWebAPI.Domain.Enums;

namespace BankingWebAPI.Application.DTOs
{
    public class DepositResponseDto
    {
        public string AccountNumber { get; set; }
        public CurrencyTypes Currency { get; set; }
        public string Message { get; set; }
        public decimal NewBalance { get; set; }
    }
}