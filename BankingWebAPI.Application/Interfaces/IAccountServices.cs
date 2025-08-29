

using BankingWebAPI.Application.DTOs;
using BankingWebAPI.Domain.Entities;
using BankingWebAPI.Domain.Enums;


namespace BankingWebAPI.Application.Interfaces
{
    public interface IAccountServices
    {
        Task<Account?> OpenAccountAsync(UserDTO userDto, AccountType accountType);
    }
}
