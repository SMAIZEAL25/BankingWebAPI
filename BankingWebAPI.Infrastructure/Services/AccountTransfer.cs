using AutoMapper;
using BankingWebAPI.Application.DTOs;
using BankingWebAPI.Application.Interfaces;
using BankingWebAPI.Application.Response;
using BankingWebAPI.Infrastructure.Integration.Response;
using BankingWebAPI.Infrastructure.Services;
using BankingWebAPI.Infrastructure.Services.Interfaces;
using BankingWebAPI.Infrastruture.Redis;
using System.Transactions;

namespace BankingWebAPI.Infrastruture.Services
{
    public class AccountTransfer : IAccountTransfer
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBankingService _bankingService;    // ← add this
        private readonly ICacheService _cache;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IMapper _mapper;

        private const string CachePrefix = "account_";
        private const int CacheExpiryHours = 20;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromHours(CacheExpiryHours);

        public AccountTransfer(
            IUnitOfWork unitOfWork,
            IBankingService bankingService,                     // ← add this
            ICacheService cache,
            IPaymentGateway paymentGateway,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _bankingService = bankingService;                   // ← add this
            _cache = cache;
            _paymentGateway = paymentGateway;
            _mapper = mapper;
        }

        private static string GetCacheKey(string accountNumber) => $"{CachePrefix}{accountNumber}";

        public async Task<CustomResponse<TransferResult>> AccountTransferAsync(TransferRequestDto dto)
        {
            var sourceAccount = await _bankingService.GetAccountCachedAsync(dto.AccountNumber);
            var destinationAccount = await _bankingService.GetAccountCachedAsync(dto.BeneficiaryAccountNumber.ToString());

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (destinationAccount != null)
            {
                sourceAccount.CurrentBalance -= dto.Amount;
                destinationAccount.CurrentBalance += dto.Amount;

                await _unitOfWork.Accounts.UpdateAsync(sourceAccount);
                await _unitOfWork.Accounts.UpdateAsync(destinationAccount);

                await _bankingService.CacheAccountDetails(sourceAccount);           // ← changed
                await _bankingService.CacheAccountDetails(destinationAccount);      // ← changed

                await _unitOfWork.CommitAsync();
                transaction.Complete();

                return CustomResponse<TransferResult>.Success(new TransferResult
                {
                    IsSuccesTransfer = true,
                    SourceAccount = sourceAccount,
                    BeneficaryAccount = destinationAccount,
                    Message = $"Transfer completed. New balance: {sourceAccount.CurrentBalance:N2}"
                });
            }

            // External transfer
            sourceAccount.CurrentBalance -= dto.Amount;
            await _unitOfWork.Accounts.UpdateAsync(sourceAccount);
            await _bankingService.CacheAccountDetails(sourceAccount);               // ← changed

            var paymentResult = await _paymentGateway.InitializeTransaction(new PaymentRequest
            {
                // ...
            });

            if (paymentResult != null && paymentResult.Status)
            {
                await _unitOfWork.CommitAsync();
                transaction.Complete();
                // ...
            }

            // Revert on failure
            await _bankingService.RevertTransfer(sourceAccount, dto.Amount);        // ← changed
            return CustomResponse<TransferResult>.FailedDependency("External transfer failed. Amount credited back");
        }
    }
}
