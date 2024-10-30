using Exc.Banking;
using System.Text.Json;

namespace Exc.Api.Outbox;

// Use PostgreSql on PROD
public class ListOutboxTransactionRepository : IOutboxTransactionRepository
{
    private readonly List<RepositoryRecord> _repository = new();

    private readonly ILogger<ListOutboxTransactionRepository> _logger;

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public ListOutboxTransactionRepository(ILogger<ListOutboxTransactionRepository> logger)
    {
        _logger = logger;
    }

    public async Task Add(Transaction transaction)
    {
        await _semaphore.WaitAsync();

        _repository.Add(new RepositoryRecord(transaction, DateTime.Now));

        _semaphore.Release();

        _logger.LogInformation($"[Add] {JsonSerializer.Serialize(transaction)}");
    }

    public async Task<int> GetTotalUnprocessedTransactions(Guid userId, Guid bankId)
    {
        await _semaphore.WaitAsync();

        var totalCount = _repository.Count(record => record.Transaction.UserId == userId && record.Transaction.BankId == bankId);
        _semaphore.Release();

        _logger.LogInformation($"[GetTotalUnprocessedTransactions] ({userId} {bankId}) {totalCount}");

        return totalCount;
    }

    public async Task SetProduced(Guid transactionId)
    {
        await _semaphore.WaitAsync();

        var transaction = _repository
            .FirstOrDefault(record => record.Transaction.Id == transactionId);

        if (transaction != null)
        {
            transaction.State = TransactionOutboxState.Produced;
        }

        _semaphore.Release();

        _logger.LogInformation($"[SetProduced] {transactionId}");
    }

    public async Task<IReadOnlyCollection<UserTransactionBatch>> GetOutboxData(int count)
    {
        await _semaphore.WaitAsync();

        var records = _repository
            .Where(record => record.State == TransactionOutboxState.Registered)
            .OrderBy(record => record.MomentIn)
            .Take(count)
            .ToArray();

        var result = records
            .GroupBy(record => (record.Transaction.UserId, record.Transaction.BankId))
            .Select(group =>
            {
                var transactions = group
                    .Select(groupItem => groupItem.Transaction)
                    .ToArray();

                return new UserTransactionBatch(transactions, group.Key.UserId, group.Key.BankId);
            })
            .ToArray();

        _semaphore.Release();

        if (result.Length > 0)
        {
            _logger.LogInformation($"[GetOutboxData] {result.Length} {JsonSerializer.Serialize(result)}");
        }

        return result;
    }

    public async Task Delete(Guid transactionId)
    {
        await _semaphore.WaitAsync();

        _repository.RemoveAll(record => record.Transaction.Id == transactionId);

        _semaphore.Release();

        _logger.LogInformation($"[Delete] {transactionId}");
    }

    private class RepositoryRecord
    {
        public RepositoryRecord(Transaction transaction, DateTime momentIn)
        {
            Transaction = transaction;
            MomentIn = momentIn;
            State = TransactionOutboxState.Registered;
        }

        public Transaction Transaction { get; }

        public DateTime MomentIn { get; }

        public TransactionOutboxState State { get; set; }
    }
}
