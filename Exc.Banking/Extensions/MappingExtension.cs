using Exc.Banking.Infrastructure.Dto;

namespace Exc.Banking;

// To Automapper
public static class MappingExtensions
{
    public static Transaction ToDomain(this TransactionDto transactionDto)
    {
        return new Transaction(transactionDto.Id, transactionDto.UserId, transactionDto.BankId);
    }

    public static TransactionDto ToDto(this Transaction transaction)
    {
        return new TransactionDto(transaction.Id, transaction.UserId, transaction.BankId);
    }
}
