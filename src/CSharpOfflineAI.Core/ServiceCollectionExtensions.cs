using Microsoft.Extensions.DependencyInjection;

namespace CSharpOfflineAI.Core;

/// <summary>
/// Extension methods for registering offline AI services with dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds offline AI services to the service collection
    /// </summary>
    public static IServiceCollection AddOfflineAI(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSingleton<IOfflineAIService, LocalOfflineAIService>();

        return services;
    }

    /// <summary>
    /// Adds a specific offline agent to the service collection
    /// </summary>
    public static IServiceCollection AddOfflineAgent<TAgent>(this IServiceCollection services)
        where TAgent : class, IOfflineAgent
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddTransient<IOfflineAgent, TAgent>();

        return services;
    }
}
