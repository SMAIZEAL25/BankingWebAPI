


using BankingWebAPI.Application.DTOs;


namespace BankingWebAPI.Response
{

    public class AccountOpeningResponse
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
        public AccountDTO Account { get; set; }
    }

}
