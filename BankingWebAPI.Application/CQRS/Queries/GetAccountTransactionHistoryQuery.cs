
using BankingWebAPI.Application.DTOs;
using BankingWebAPI.Application.Response;
using MediatR;

namespace BankingWebAPI.Application.Queries
{
    public record GetAccountTransactionHistoryQuery(string AccountNumber)
        : IRequest<CustomResponse<IEnumerable<TransactionHistoryDto>>>;

    
}
