using Exc.Api.EventBus.Abstractions;
using Exc.Api.Outbox;

namespace Exc.Api.EventBus;

public class TresholdLongtermDetector : ILongtermDetector
{
    private const int _longtermTreshold = 1;

    private IOutboxTransactionRepository _repository;

    public TresholdLongtermDetector(IOutboxTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> IsLongterm(Guid userId, Guid bankId)
    {
        var unprocessedCount = await _repository.GetTotalUnprocessedTransactions(userId, bankId);

        return _longtermTreshold < unprocessedCount;
    }
}
