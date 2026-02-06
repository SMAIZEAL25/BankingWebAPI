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
        IRepository<Account> Account { get; }           // consider renaming to Accounts for consistency
        IAccountRepository Accounts { get; }
        IViewAccountBalance ViewAccountBalance { get; }  // if this is truly a repository/view
        IAccountingHistoryRepository GetAccountingHistory { get; }

        Task<int> CommitAsync();
    }
}
