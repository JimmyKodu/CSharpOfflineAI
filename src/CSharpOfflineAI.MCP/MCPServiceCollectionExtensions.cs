using Microsoft.Extensions.DependencyInjection;

namespace CSharpOfflineAI.MCP;

/// <summary>
/// Extension methods for registering MCP services with dependency injection
/// </summary>
public static class MCPServiceCollectionExtensions
{
    /// <summary>
    /// Adds MCP server services to the service collection
    /// </summary>
    public static IServiceCollection AddMCPServer(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSingleton<IMCPServer, OfflineMCPServer>();

        return services;
    }
}
