using Exc.Banking.Infrastructure;
using Exc.TransactionProcessor.Banking;

namespace Exc.TransactionProcessor.EventBus;

public class ConsumerJobFactory : IConsumerJobFactory
{
    private readonly ILogger<ConsumerJob> _logger;
    private readonly ITransactionResultPublisher _publisher;
    private readonly IBankAdapterResolver _bankAdapterResolver;
    private readonly IQueueNameBuilder _queueNameBuilder;

    public ConsumerJobFactory(
        ILogger<ConsumerJob> logger,
        ITransactionResultPublisher publisher,
        IBankAdapterResolver bankAdapterResolver,
        IQueueNameBuilder queueNameBuilder)
    {
        _logger = logger;
        _publisher = publisher;
        _bankAdapterResolver = bankAdapterResolver;
        _queueNameBuilder = queueNameBuilder;
    }

    public IConsumerJob Create(Guid bankId)
    {
        if (_bankAdapterResolver.TryGetAdapter(bankId, out var bankAdapter))
        {
            var longtermQueue = _queueNameBuilder.GetRequestQueueName(bankId, true);
            var shorttermQueue = _queueNameBuilder.GetRequestQueueName(bankId, false);
            return new ConsumerJob(_logger, _publisher, bankAdapter, longtermQueue, shorttermQueue);
        }

        throw new ArgumentException($"Invalid bankId: {bankId}");
    }
}
