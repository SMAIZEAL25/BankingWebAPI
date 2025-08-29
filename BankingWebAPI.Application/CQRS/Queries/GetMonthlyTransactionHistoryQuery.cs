
using BankingWebAPI.Application.DTOs;
using BankingWebAPI.Application.Response;
using MediatR;

namespace BankingWebAPI.Application.Queries
{
    

    public record GetMonthlyTransactionStatementQuery(string AccountNumber, int NumberOfMonths)
        : IRequest<CustomResponse<IEnumerable<TransactionHistoryDto>>>;
}
