using Exc.Banking;

namespace Exc.TransactionProcessor.Banking;

public class AlfaBankAdapter : IBankAdapter
{ 
    public AlfaBankAdapter(IBankRepository bankRepository)
    {
        Bank = bankRepository.AlfaBank;
    }

    public Bank Bank { get;}

    public async Task PocessTransactionAsync(Transaction transaction)
    {
        await Task.Delay(5000);
    }
}
