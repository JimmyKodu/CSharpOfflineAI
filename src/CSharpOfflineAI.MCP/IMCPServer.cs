namespace CSharpOfflineAI.MCP;

/// <summary>
/// Interface for MCP server implementation that works offline
/// </summary>
public interface IMCPServer
{
    /// <summary>
    /// Starts the MCP server
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the MCP server
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes an MCP message
    /// </summary>
    Task<MCPMessage> ProcessMessageAsync(MCPMessage message, CancellationToken cancellationToken = default);
}
