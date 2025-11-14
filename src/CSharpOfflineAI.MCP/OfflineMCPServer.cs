using CSharpOfflineAI.Core;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CSharpOfflineAI.MCP;

/// <summary>
/// Offline MCP server implementation that handles Model Context Protocol communication
/// </summary>
public class OfflineMCPServer : IMCPServer
{
    private readonly IOfflineAIService _aiService;
    private readonly ILogger<OfflineMCPServer> _logger;
    private bool _isRunning;

    public OfflineMCPServer(IOfflineAIService aiService, ILogger<OfflineMCPServer> logger)
    {
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Offline MCP Server...");

        if (!_aiService.IsInitialized)
        {
            await _aiService.InitializeAsync(cancellationToken);
        }

        _isRunning = true;
        _logger.LogInformation("Offline MCP Server started successfully");
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping Offline MCP Server...");
        _isRunning = false;
        _logger.LogInformation("Offline MCP Server stopped");
        return Task.CompletedTask;
    }

    public async Task<MCPMessage> ProcessMessageAsync(MCPMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (!_isRunning)
        {
            return CreateErrorResponse(message.Id, -32000, "Server is not running");
        }

        _logger.LogInformation("Processing MCP message: {Method}", message.Method);

        try
        {
            return message.Method switch
            {
                "initialize" => await HandleInitializeAsync(message, cancellationToken),
                "completion" => await HandleCompletionAsync(message, cancellationToken),
                "tools/list" => HandleToolsList(message),
                "ping" => HandlePing(message),
                _ => CreateErrorResponse(message.Id, -32601, $"Method not found: {message.Method}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MCP message");
            return CreateErrorResponse(message.Id, -32603, $"Internal error: {ex.Message}");
        }
    }

    private async Task<MCPMessage> HandleInitializeAsync(MCPMessage message, CancellationToken cancellationToken)
    {
        var result = new
        {
            protocolVersion = "2024-11-05",
            serverInfo = new
            {
                name = "CSharpOfflineAI",
                version = "1.0.0"
            },
            capabilities = new
            {
                tools = new { },
                prompts = new { },
                resources = new { }
            }
        };

        return new MCPMessage
        {
            Id = message.Id,
            Result = JsonSerializer.SerializeToElement(result)
        };
    }

    private async Task<MCPMessage> HandleCompletionAsync(MCPMessage message, CancellationToken cancellationToken)
    {
        if (!message.Params.HasValue)
        {
            return CreateErrorResponse(message.Id, -32602, "Invalid params");
        }

        var paramsObj = message.Params.Value;
        string prompt = string.Empty;

        if (paramsObj.TryGetProperty("prompt", out var promptElement))
        {
            prompt = promptElement.GetString() ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            return CreateErrorResponse(message.Id, -32602, "Prompt is required");
        }

        var response = await _aiService.GenerateResponseAsync(prompt, cancellationToken);

        var result = new
        {
            completion = response,
            model = _aiService.GetModelInfo(),
            stopReason = "end_turn"
        };

        return new MCPMessage
        {
            Id = message.Id,
            Result = JsonSerializer.SerializeToElement(result)
        };
    }

    private MCPMessage HandleToolsList(MCPMessage message)
    {
        var result = new
        {
            tools = new[]
            {
                new
                {
                    name = "generate_text",
                    description = "Generate text using the offline AI model",
                    inputSchema = new
                    {
                        type = "object",
                        properties = new
                        {
                            prompt = new { type = "string", description = "The input prompt" }
                        },
                        required = new[] { "prompt" }
                    }
                }
            }
        };

        return new MCPMessage
        {
            Id = message.Id,
            Result = JsonSerializer.SerializeToElement(result)
        };
    }

    private MCPMessage HandlePing(MCPMessage message)
    {
        var result = new { status = "ok", timestamp = DateTime.UtcNow };

        return new MCPMessage
        {
            Id = message.Id,
            Result = JsonSerializer.SerializeToElement(result)
        };
    }

    private MCPMessage CreateErrorResponse(string? id, int code, string message)
    {
        return new MCPMessage
        {
            Id = id,
            Error = new MCPError
            {
                Code = code,
                Message = message
            }
        };
    }
}
