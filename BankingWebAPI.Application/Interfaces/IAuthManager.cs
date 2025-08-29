

using BaningWebAPI.Application.DTOs;
using BankingWebAPI.Application.DTOs;

using System.Threading.Tasks;

namespace BankingWebAPI.Application.Interfaces
{
    public interface IAuthManager
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
      
        Task<RegistrationResponse> RegisterAsync(RegisterRequest request, string role = "Reader");
    }
}
