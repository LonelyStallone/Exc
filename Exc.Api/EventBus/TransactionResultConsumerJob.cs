using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Exc.Api.Outbox;
using Exc.Banking.Infrastructure.Dto;
using Exc.Banking.Infrastructure;

public class TransactionResultConsumerJob : IHostedService
{
    // To Options
    private const string _hostName = "localhost";


    private IConnection _connection;
    private IModel _channel;

    private readonly ILogger<TransactionResultConsumerJob> _logger;
    private readonly IOutboxTransactionRepository _repository;
    private readonly IQueueNameBuilder _queueNameBuilder;

    public TransactionResultConsumerJob(
        ILogger<TransactionResultConsumerJob> logger,
        IOutboxTransactionRepository repository,
        IQueueNameBuilder queueNameBuilder)
    {
        _logger = logger;
        _repository = repository;
        _queueNameBuilder = queueNameBuilder;

        var factory = new ConnectionFactory { HostName = _hostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        Initialize();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5000); //warmup wait

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (ch, ea) =>
        {
            var dto = Deserialize(ea.Body.ToArray());

            _logger.LogInformation($"Produce to: {ea.RoutingKey}, {JsonSerializer.Serialize(dto)}");

            await _repository.Delete(dto.Transaction.Id);

            // if(!dto.IsSuccess) handleError

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(_queueNameBuilder.ResponseQueueName, false, consumer);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Close();
        _connection.Close();

        return Task.CompletedTask;
    }

    private void Initialize()
    {
        _channel.ExchangeDeclare(
            exchange: _queueNameBuilder.ExchangeName,
            type: "direct",
            durable: true,
            autoDelete: false,
            arguments: null);

        _channel.QueueDeclare(
            queue: _queueNameBuilder.ResponseQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    private TransactionProcessingResultDto Deserialize(byte[] rawTransaction)
    {
        var transactionText = Encoding.UTF8.GetString(rawTransaction);
        return JsonSerializer.Deserialize<TransactionProcessingResultDto>(transactionText)!;
    }
}