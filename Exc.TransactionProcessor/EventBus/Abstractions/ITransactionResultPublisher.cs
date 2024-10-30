using Exc.Banking;

namespace Exc.TransactionProcessor.EventBus;

public interface ITransactionResultPublisher
{
    Task PublishAsync(Transaction transaction, bool isSuccess, CancellationToken token);
}
