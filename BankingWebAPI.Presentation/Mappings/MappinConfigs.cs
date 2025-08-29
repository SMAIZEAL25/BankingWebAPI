using AutoMapper;
using BankingApp.Domain.Entities;
using BankingWebAPI.Application.DTOs;
using BankingWebAPI.Domain.Entities;
using BankingWebAPI.Infrastructure.Integration.Response;

namespace BankingWebAPI.Mappings
{

    public class MappinConfigs : Profile
    {
        public MappinConfigs()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<Transaction, TransactionData>().ReverseMap();

        }
    }
}

