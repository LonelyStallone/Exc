using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Exc.Banking.Infrastructure.Dto;
using Exc.Banking.Infrastructure;
using Exc.Banking;

public class WarmUpJob : IHostedService
{
    // To Options
    private const string _hostName = "localhost";

    private readonly IBankRepository _repository;
    private readonly IQueueNameBuilder _queueNameBuilder;

    public WarmUpJob(IBankRepository repository, IQueueNameBuilder queueNameBuilder)
    {
        _repository = repository;
        _queueNameBuilder = queueNameBuilder;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: _queueNameBuilder.ExchangeName,
            type: "direct",
            durable: true,
            autoDelete: false,
            arguments: null);

        InitializeQueue(channel, _queueNameBuilder.ExchangeName, _queueNameBuilder.ResponseQueueName);

        var banks = await _repository.GetBanksAsync(cancellationToken);

        foreach (var bank in banks)
        {
            var longtermQueueName = _queueNameBuilder.GetRequestQueueName(bank.Id, true);
            InitializeQueue(channel, _queueNameBuilder.ExchangeName, longtermQueueName);
            var shorttermQueueName = _queueNameBuilder.GetRequestQueueName(bank.Id, false);
            InitializeQueue(channel, _queueNameBuilder.ExchangeName, shorttermQueueName);
        }

        channel.Close();
        connection.Close();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void InitializeQueue(IModel channel, string exchangeName, string queueName)
    {
        channel.ExchangeDeclare(
            exchange: exchangeName,
            type: "direct",
            durable: true,
            autoDelete: false,
            arguments: null);

        channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.QueueBind(
            queue: queueName,
            exchange: exchangeName,
            routingKey: queueName);
    }

    private TransactionProcessingResultDto Deserialize(byte[] rawTransaction)
    {
        var transactionText = Encoding.UTF8.GetString(rawTransaction);
        return JsonSerializer.Deserialize<TransactionProcessingResultDto>(transactionText)!;
    }
}