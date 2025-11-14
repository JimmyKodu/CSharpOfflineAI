using Microsoft.Extensions.AI;

namespace CSharpOfflineAI.Core;

/// <summary>
/// Interface for offline AI services that can run without internet connectivity
/// </summary>
public interface IOfflineAIService
{
    /// <summary>
    /// Gets whether the service is properly initialized and ready for offline use
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Initializes the offline AI service with local models
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a response using the offline AI model
    /// </summary>
    Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about the loaded model
    /// </summary>
    string GetModelInfo();
}
