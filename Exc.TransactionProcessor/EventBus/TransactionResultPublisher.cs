using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Exc.Banking;
using Exc.Banking.Infrastructure;
using Exc.Banking.Infrastructure.Dto;

namespace Exc.TransactionProcessor.EventBus;

public class TransactionResultPublisher : ITransactionResultPublisher
{
    // To Options
    private const string _hostName = "localhost";
    private readonly IQueueNameBuilder _queueNameBuilder;

    public TransactionResultPublisher(IQueueNameBuilder queueNameBuilder)
    {
        _queueNameBuilder = queueNameBuilder;
    }

    public Task PublishAsync(Transaction transaction, bool isSuccess, CancellationToken token)
    {
        var queueName = _queueNameBuilder.ResponseQueueName;
        var exchangeName = _queueNameBuilder.ExchangeName;

        var factory = new ConnectionFactory() { HostName = _hostName };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
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

            // channel.QueueBind(
            //     queue: queueName,
            //     exchange: exchangeName,
            //     routingKey: queueName);

            var body = Serialize(transaction, isSuccess);

            channel.BasicPublish(exchange: exchangeName,
                routingKey: queueName,
                basicProperties: null,
                body: body);

        }

        return Task.CompletedTask;
    }

    private byte[] Serialize(Transaction transaction, bool isSuccess)
    {
        var dto = new TransactionProcessingResultDto(transaction.ToDto(), isSuccess);

        var message = JsonSerializer.Serialize(dto);
        return Encoding.UTF8.GetBytes(message);
    }
}