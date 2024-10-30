namespace Exc.Banking.Infrastructure;

public interface IQueueNameBuilder
{
    string ExchangeName { get; }

    string ResponseQueueName { get; } 

    string GetRequestQueueName(Guid bankId, bool isLongterm);
}
