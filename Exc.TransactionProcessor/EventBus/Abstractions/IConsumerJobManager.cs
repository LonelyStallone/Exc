namespace Exc.TransactionProcessor.EventBus;

public interface IConsumerJobManager : IDisposable
{
    Task StartAsync(CancellationToken cancellationToken);
}
