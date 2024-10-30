using Exc.Banking;

namespace Exc.Api.EventBus;

public interface ITransactionPublisher
{
    Task Publish(Guid bankId, bool isLongterm, IReadOnlyCollection<Transaction> transactions);
}
