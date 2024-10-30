using Exc.Banking;
using Exc.Banking.Infrastructure.Dto;
using Exc.TransactionProcessor.Banking;
using Exc.TransactionProcessor.EventBus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class ConsumerJob : IConsumerJob
{
    private const string _hostName = "localhost";

    private bool _disposed = false;

    private readonly ILogger<ConsumerJob> _logger;
    private readonly ITransactionResultPublisher _resultPublisher;
    private readonly IBankAdapter _bankAdapter;
    private readonly string _longtermQueueName;
    private readonly string _shorttermQueueName;

    private IConnection _connection;
    private IModel _channel;



    public ConsumerJob(
        ILogger<ConsumerJob> logger,
        ITransactionResultPublisher resultPublisher,
        IBankAdapter bankAdapter,
        string longtermQueueName,
        string shorttermQueueName)
    {
        _logger = logger;
        _resultPublisher = resultPublisher;
        _bankAdapter = bankAdapter;
        _longtermQueueName = longtermQueueName;
        _shorttermQueueName = shorttermQueueName;

        var factory = new ConnectionFactory { HostName = _hostName };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _channel.Close();
            _connection.Close();
            _disposed = true;
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _channel.BasicQos(0, 1, true);

        var consumer = CreateConsumer();
        _channel.BasicConsume(_longtermQueueName, false, consumer);
        _channel.BasicConsume(_shorttermQueueName, false, consumer);

        return Task.CompletedTask;
    }

    private IBasicConsumer CreateConsumer()
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (ch, ea) =>
        {
            var dto = Deserialize(ea.Body.ToArray());
            var transaction = dto.ToDomain();

            try
            {
                _logger.LogInformation($"Consume from: {ea.RoutingKey}, {JsonSerializer.Serialize(transaction)}");
                await _bankAdapter.PocessTransactionAsync(transaction);
                await _resultPublisher.PublishAsync(transaction, true, CancellationToken.None);
            }
            catch (Exception)
            {
                await _resultPublisher.PublishAsync(transaction, false, CancellationToken.None);
            }
         
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        return consumer;
    }


    private TransactionDto Deserialize(byte[] rawTransaction)
    {
        var transactionText = Encoding.UTF8.GetString(rawTransaction);
        return JsonSerializer.Deserialize<TransactionDto>(transactionText)!;
    }
}