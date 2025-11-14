using Microsoft.Extensions.Logging;

namespace CSharpOfflineAI.Core;

/// <summary>
/// Base implementation of an offline agent that follows Microsoft Agent Framework principles
/// </summary>
public class OfflineAgentBase : IOfflineAgent
{
    private readonly IOfflineAIService _aiService;
    private readonly ILogger<OfflineAgentBase> _logger;

    public string Id { get; }
    public string Name { get; }
    public string Description { get; }

    public OfflineAgentBase(
        string id,
        string name,
        string description,
        IOfflineAIService aiService,
        ILogger<OfflineAgentBase> logger)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual async Task<AgentResponse> ExecuteAsync(AgentRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _logger.LogInformation("Agent {AgentName} executing request", Name);

        try
        {
            if (!_aiService.IsInitialized)
            {
                await _aiService.InitializeAsync(cancellationToken);
            }

            var response = await _aiService.GenerateResponseAsync(request.Input, cancellationToken);

            var metadata = new Dictionary<string, object>
            {
                ["agent_id"] = Id,
                ["agent_name"] = Name,
                ["model_info"] = _aiService.GetModelInfo(),
                ["timestamp"] = DateTime.UtcNow
            };

            _logger.LogInformation("Agent {AgentName} completed request successfully", Name);

            return new AgentResponse(response, true, null, metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agent {AgentName} failed to execute request", Name);
            return new AgentResponse(string.Empty, false, ex.Message);
        }
    }
}
