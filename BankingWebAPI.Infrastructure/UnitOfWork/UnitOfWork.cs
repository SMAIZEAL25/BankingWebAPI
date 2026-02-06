
using BankingApp.Domain.Entities;
using BankingApp.Infrastructure.Database;
using BankingWebAPI.Application.Interfaces;
using BankingWebAPI.Domain.Entities;


namespace BankingWebAPI.Infrastructure.UnitOfWork
{

    public class UnitOfWork : IUnitOfWork
{
    private readonly BankingDbContext _context;

    public IRepository<User> Users { get; }
    public IRepository<Transaction> Transactions { get; }
    public IRepository<Account> Account { get; }
    public IAccountRepository Accounts { get; }
    public IViewAccountBalance ViewAccountBalance { get; }
    public IAccountingHistoryRepository GetAccountingHistory { get; }

    public UnitOfWork(
        BankingDbContext context,
        IRepository<User> userRepo,
        IRepository<Transaction> transactionRepo,
        IRepository<Account> account,
        IAccountRepository accountRepo,
        IViewAccountBalance viewAccountBalance,
        IAccountingHistoryRepository accountingHistoryRepo)
    {
        _context = context;
        Users = userRepo;
        Transactions = transactionRepo;
        Account = account;
        Accounts = accountRepo;
        ViewAccountBalance = viewAccountBalance;
        GetAccountingHistory = accountingHistoryRepo;
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
}

