

using BankingApp.Domain.Entities;
using BankingWebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingWebAPI.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Transaction> Transactions { get; }
        IRepository<Account> Account { get; }
        IAccountRepository Accounts { get; }
        IBankingService BankingService { get; }
        IAccountServices AccountServices { get; }
        IViewAccountBalance ViewAccountBalance { get; }
        IAccountingHistoryRepository GetAccountingHistory { get; }

        Task<int> CommitAsync(); 
    }
}
