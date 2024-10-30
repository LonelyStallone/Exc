using Exc.Banking;

namespace Exc.Api.Outbox;

public interface IOutboxTransactionRepository
{
    Task Add(Transaction transaction);

    Task<int> GetTotalUnprocessedTransactions(Guid userId, Guid bankId);

    Task SetProduced(Guid transactionId);

    Task Delete(Guid transactionId);

    Task<IReadOnlyCollection<UserTransactionBatch>> GetOutboxData(int count);
}
