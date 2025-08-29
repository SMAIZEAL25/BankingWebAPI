
using System.Collections.Concurrent;


namespace BankingWebAPI.Application.Interfaces
{
    public interface IRateLimiter
    {
        bool IsRequestAllowed(string clientIdentifier);
    }
}

   