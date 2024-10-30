using Exc.Banking;

namespace Exc.TransactionProcessor.Banking;

public class SberBankAdapter : IBankAdapter
{
    public SberBankAdapter(IBankRepository bankRepository)
    {
        Bank = bankRepository.SberBank;
    }

    public Bank Bank { get; }

    public async Task PocessTransactionAsync(Transaction transaction)
    {
        await Task.Delay(5000);
    }
}
