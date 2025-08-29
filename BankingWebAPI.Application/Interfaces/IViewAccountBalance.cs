

using BankingWebAPI.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingWebAPI.Application.Interfaces
{
    public interface IViewAccountBalance
    {
        Task<AccountBalanceResponseDto> ViewAccountBalanceAsync(string accountNumber);
        Task<AccountDetailsResponseDto> ViewAccountDetailsAsync(string accountNumber);
    }
}
