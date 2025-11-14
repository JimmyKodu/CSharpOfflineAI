using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace CSharpOfflineAI.Core;

/// <summary>
/// Implementation of offline AI service using local models
/// This service operates without requiring internet connectivity
/// </summary>
public class LocalOfflineAIService : IOfflineAIService
{
    private readonly ILogger<LocalOfflineAIService> _logger;
    private bool _isInitialized;
    private string _modelPath;

    public bool IsInitialized => _isInitialized;

    public LocalOfflineAIService(ILogger<LocalOfflineAIService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _modelPath = string.Empty;
    }

    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing offline AI service...");

        try
        {
            // In a real implementation, this would load a local AI model
            // For demonstration, we're simulating the initialization
            _modelPath = "local-model-v1.0";
            _isInitialized = true;

            _logger.LogInformation("Offline AI service initialized successfully with model: {ModelPath}", _modelPath);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize offline AI service");
            throw;
        }
    }

    public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("AI service is not initialized. Call InitializeAsync first.");
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));
        }

        _logger.LogInformation("Generating response for prompt (length: {Length})", prompt.Length);

        // Simulate processing time
        await Task.Delay(100, cancellationToken);

        // In a real implementation, this would use Microsoft.Extensions.AI to interact with a local model
        // For demonstration purposes, we're returning a simulated response
        var response = $"[Offline AI Response] Processed prompt: '{prompt}' using local model '{_modelPath}'";

        _logger.LogInformation("Response generated successfully");
        return response;
    }

    public string GetModelInfo()
    {
        if (!_isInitialized)
        {
            return "Model not initialized";
        }

        return $"Local Offline Model: {_modelPath}, Status: Ready, Mode: Offline";
    }
}
