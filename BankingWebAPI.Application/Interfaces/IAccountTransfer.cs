
using BankingWebAPI.Application.DTOs;
using BankingWebAPI.Application.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingWebAPI.Application.Interfaces
{
    public interface IAccountTransfer
    {
        Task<CustomResponse<TransferResult>> AccountTransferAsync(TransferRequestDto transferRequestDto);
    }
}
