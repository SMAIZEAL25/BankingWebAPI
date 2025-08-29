using AutoMapper;
using BankingApp.Domain.Entities;
using BankingWebAPI.Application.DTOs;
using BankingWebAPI.Domain.Entities;
using BankingWebAPI.Infrastructure.Integration.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApp.Application.Mappings
{
    public class MappinConfigs : Profile
    {
        public MappinConfigs()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Account, AccountDTO>().ReverseMap();
            CreateMap<Transaction, TransactionData>().ReverseMap();
            CreateMap<RegisterRequest, User>().ReverseMap();
            CreateMap<Transaction, TransactionHistoryDto>().ReverseMap();
            CreateMap<Account, DepositRequestDto>().ReverseMap();
            CreateMap<Account, WithdrawRequestDto>().ReverseMap();
            CreateMap<Account, TransferRequestDto>().ReverseMap();
            

        }
    }
}
