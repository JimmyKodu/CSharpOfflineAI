using Microsoft.Extensions.AI;

namespace CSharpOfflineAI.Core;

/// <summary>
/// Represents an AI agent that can perform tasks offline using the Microsoft Agent Framework concepts
/// </summary>
public interface IOfflineAgent
{
    /// <summary>
    /// Gets the agent's unique identifier
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the agent's name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the agent's description
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes a task using the agent
    /// </summary>
    Task<AgentResponse> ExecuteAsync(AgentRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a request to an offline agent
/// </summary>
public record AgentRequest(string Input, Dictionary<string, object>? Context = null);

/// <summary>
/// Represents a response from an offline agent
/// </summary>
public record AgentResponse(string Output, bool Success, string? ErrorMessage = null, Dictionary<string, object>? Metadata = null);
