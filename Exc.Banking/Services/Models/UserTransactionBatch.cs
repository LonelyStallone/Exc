namespace Exc.Banking;

public sealed record UserTransactionBatch(IReadOnlyCollection<Transaction> Transactions, Guid UserId, Guid BankId);