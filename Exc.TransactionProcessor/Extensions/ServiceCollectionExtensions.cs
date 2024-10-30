using Exc.TransactionProcessor.Banking;
using Exc.TransactionProcessor.EventBus;

namespace Exc.TransactionProcessor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTransactionProcessor(this IServiceCollection services)
    {
        return services
            .AddBankingAdapters()
            .AddInfrastructure();
    }

    private static IServiceCollection AddBankingAdapters(this IServiceCollection services)
    {
        return services
            .AddSingleton<IBankAdapterResolver, BankAdapterResolver>()
            .AddSingleton<IBankAdapter, AlfaBankAdapter>()
            .AddSingleton<IBankAdapter, SberBankAdapter>();
    }
    private static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddSingleton<IConsumerJobFactory, ConsumerJobFactory>()
            .AddSingleton<ITransactionResultPublisher, TransactionResultPublisher>()
            .AddHostedService<ConsumerJobManager>();
    }
}
