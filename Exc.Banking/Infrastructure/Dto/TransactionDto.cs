namespace Exc.Banking.Infrastructure.Dto;

public sealed record TransactionDto(Guid Id, Guid UserId, Guid BankId);