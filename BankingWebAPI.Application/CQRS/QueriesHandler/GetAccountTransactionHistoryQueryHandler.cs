using BankingWebAPI.Application.DTOs;
using BankingWebAPI.Application.Queries;
using BankingWebAPI.Application.Response;

using BankingWebAPI.Application.Interfaces;
using MediatR;

namespace BankingApp.Application.Handlers
{
    public class GetAccountTransactionHistoryQueryHandler
        : IRequestHandler<GetAccountTransactionHistoryQuery, CustomResponse<IEnumerable<TransactionHistoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAccountTransactionHistoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomResponse<IEnumerable<TransactionHistoryDto>>> Handle(
            GetAccountTransactionHistoryQuery request,
            CancellationToken cancellationToken)
        {
           
            return await _unitOfWork.GetAccountingHistory
                .GetAccountTransactionHistoryAsync(request.AccountNumber);
        }
    }

    public class GetMonthlyTransactionStatementQueryHandler
        : IRequestHandler<GetMonthlyTransactionStatementQuery, CustomResponse<IEnumerable<TransactionHistoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMonthlyTransactionStatementQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomResponse<IEnumerable<TransactionHistoryDto>>> Handle(
            GetMonthlyTransactionStatementQuery request,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.GetAccountingHistory
                .GetMonthlyTransactionStatementAsync(request.AccountNumber, request.NumberOfMonths);
        }
    }
}
