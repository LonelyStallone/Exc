using Exc.Api.EventBus;
using Exc.Api.EventBus.Abstractions;
using Exc.Api.Outbox;
using System.Text.Json;

public class OutboxPublisherJob : IHostedService
{
    private const int _totalSize = 10;

    private readonly IOutboxTransactionRepository _repository;
    private readonly ITransactionPublisher _publisher;
    private readonly ILogger<OutboxPublisherJob> _logger;
    private readonly ILongtermDetector _longtermDetector;

    public OutboxPublisherJob(
        IOutboxTransactionRepository repository,
        ITransactionPublisher publisher,
        ILongtermDetector longtermDetector,
        ILogger<OutboxPublisherJob> logger)
    {
        _repository = repository;
        _publisher = publisher;
        _logger = logger;
        _longtermDetector = longtermDetector;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = ExecuteAsync(cancellationToken);
        return Task.CompletedTask;
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var outboxData = await _repository.GetOutboxData(_totalSize);

            foreach (var batch in outboxData)
            {
                var isLongterm = await _longtermDetector.IsLongterm(batch.UserId, batch.BankId);
                await _publisher.Publish(batch.BankId, isLongterm, batch.Transactions);

                foreach (var transaction in batch.Transactions)
                {
                    await _repository.SetProduced(transaction.Id);
                }

                _logger.LogInformation($"[ExecuteAsync] IsLongterm: {isLongterm}. Batch: {JsonSerializer.Serialize(batch)}");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}