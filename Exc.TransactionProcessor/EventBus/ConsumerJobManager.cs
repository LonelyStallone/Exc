using Exc.Banking;

namespace Exc.TransactionProcessor.EventBus;

public class ConsumerJobManager : IHostedService
{
    private readonly IConsumerJobFactory _consumerJobFactory;
    private readonly IBankRepository _bankRepository;

    private readonly List<IConsumerJob> _jobs = new();

    public ConsumerJobManager(IConsumerJobFactory consumerJobFactory, IBankRepository bankRepository)
    {
        _consumerJobFactory = consumerJobFactory;
        _bankRepository = bankRepository;
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5000); //warmup wait

        var banks = await _bankRepository.GetBanksAsync(cancellationToken);

        foreach (var bank in banks)
        {
            var consumerJob = _consumerJobFactory.Create(bank.Id);
            _ = consumerJob.StartAsync(cancellationToken);
            _jobs.Add(consumerJob);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var job in _jobs)
        {
            job.Dispose();
        }

        return Task.CompletedTask;
    }
}
