using Exc.Api.EventBus;
using Exc.Api.EventBus.Abstractions;
using Exc.Api.Outbox;
using Microsoft.Extensions.DependencyInjection;

namespace Exc.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExcServices(this IServiceCollection services)
    {
        return services
            .AddOutbox()
            .AddEventBus()
            .AddHostedService<WarmUpJob>();
    }

    private static IServiceCollection AddOutbox(this IServiceCollection services)
    {
        return services
            .AddSingleton<IOutboxTransactionRepository, ListOutboxTransactionRepository>()
            .AddHostedService<OutboxPublisherJob>();
    }

    private static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        return services
            .AddSingleton<ITransactionPublisher, TransactionPublisher>()
            .AddSingleton<ILongtermDetector, TresholdLongtermDetector>()
            .AddHostedService<TransactionResultConsumerJob>();
    }
}
