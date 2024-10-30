using Exc.Banking.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Exc.Banking;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBanking(this IServiceCollection services)
    {
        return services
            .AddServices()
            .AddInfrastructure();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IBankRepository, BankRepository>()
            .AddSingleton<IUserRepositiry, UserRepository>();
    }
    private static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services.AddSingleton<IQueueNameBuilder, QueueNameBuilder>();
    }
}
