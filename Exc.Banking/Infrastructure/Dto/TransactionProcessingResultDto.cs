namespace Exc.Banking.Infrastructure.Dto;

public sealed record TransactionProcessingResultDto(TransactionDto Transaction, bool IsSeccess);
