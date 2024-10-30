using Exc.Banking;
using Exc.Banking.Infrastructure;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Exc.Api.EventBus;

public class TransactionPublisher : ITransactionPublisher
{
    // To Options
    private const string _hostName = "localhost";

    private readonly IQueueNameBuilder _queueNameBuilder;

    private readonly HashSet<string> _queueSet;

    public TransactionPublisher(IQueueNameBuilder queueNameBuilder)
    {
        _queueNameBuilder = queueNameBuilder;
    }

    public Task Publish(Guid bankId, bool isLongterm, IReadOnlyCollection<Transaction> transactions)
    {
        var queueName = _queueNameBuilder.GetRequestQueueName(bankId, isLongterm);
        var exchangeName = _queueNameBuilder.ExchangeName;

        var factory = new ConnectionFactory() { HostName = _hostName };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            PrepareChannel(channel, exchangeName, queueName);


            foreach (var transaction in transactions)
            {
                var body = Serialize(transaction);

                channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: queueName,
                    basicProperties: null,
                    body: body);
            }
        }

        return Task.CompletedTask;
    }

    private void PrepareChannel(IModel channel, string exchangeName, string queueName)
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

        // if (_queueSet.Contains(queueName))
        // {
        //     return;
        // }
        // 
        // channel.QueueBind(
        //     queue: queueName,
        //     exchange: exchangeName,
        //     routingKey: queueName);
        // 
        // _queueSet.Add(queueName);
    }

    private byte[] Serialize(Transaction transaction)
    {
        var dto = transaction.ToDto();

        var message = JsonSerializer.Serialize(dto);
        return Encoding.UTF8.GetBytes(message);
    }
}