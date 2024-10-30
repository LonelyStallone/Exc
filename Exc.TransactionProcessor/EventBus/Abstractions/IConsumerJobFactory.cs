namespace Exc.TransactionProcessor.EventBus;

public interface IConsumerJobFactory
{
    IConsumerJob Create(Guid bankId);
}
