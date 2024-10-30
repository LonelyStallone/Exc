namespace Exc.Banking.Infrastructure;

//from Config
internal class QueueNameBuilder : IQueueNameBuilder
{
    public string ExchangeName => "banking_transactions_exchange";

    public string GetRequestQueueName(Guid bankId, bool isLongterm)
    {
        var durationSuffix = isLongterm ? "longterm" : "shortterm";
        return $"{bankId}_{durationSuffix}";
    }

    public string ResponseQueueName => "banking_transactions_response_queue";
}
