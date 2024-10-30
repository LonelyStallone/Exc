using System.Diagnostics.CodeAnalysis;

namespace Exc.TransactionProcessor.Banking;

public interface IBankAdapterResolver
{
    bool TryGetAdapter(Guid bankId, [MaybeNullWhen(false)] out IBankAdapter bankAdapter);
}
