namespace Exc.TransactionProcessor.EventBus;

public interface IConsumerJob : IDisposable
{
    Task StartAsync(CancellationToken cancellationToken);
}
