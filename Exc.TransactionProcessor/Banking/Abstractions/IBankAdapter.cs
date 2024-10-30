using Exc.Banking;

namespace Exc.TransactionProcessor.Banking;

public interface IBankAdapter
{
    Bank Bank { get; }

    Task PocessTransactionAsync(Transaction transaction);
}
