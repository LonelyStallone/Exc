using System.Diagnostics.CodeAnalysis;

namespace Exc.TransactionProcessor.Banking;

public class BankAdapterResolver : IBankAdapterResolver
{
    private readonly Dictionary<Guid, IBankAdapter> _bankAdaptersMap;

    public BankAdapterResolver(IEnumerable<IBankAdapter> bankAdapters)
    {
        _bankAdaptersMap = bankAdapters.ToDictionary(adapter => adapter.Bank.Id);
    }

    public bool TryGetAdapter(Guid bankId, [MaybeNullWhen(false)] out IBankAdapter bankAdapter)
        => _bankAdaptersMap.TryGetValue(bankId, out bankAdapter);
}
