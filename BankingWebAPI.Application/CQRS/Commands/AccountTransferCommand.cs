using BankingWebAPI.Application.DTOs;
using BankingWebAPI.Application.Response;

using MediatR;

public record AccountTransferCommand(TransferRequestDto TransferRequest)
    : IRequest<CustomResponse<TransferResult>>;
