# Examples and Usage Guide

This document provides detailed examples of using the CSharpOfflineAI solution.

## Table of Contents

1. [Basic Setup](#basic-setup)
2. [Using Offline AI Service](#using-offline-ai-service)
3. [Creating Custom Agents](#creating-custom-agents)
4. [MCP Server Integration](#mcp-server-integration)
5. [Advanced Scenarios](#advanced-scenarios)

## Basic Setup

### Installing the Solution

```bash
# Clone the repository
git clone https://github.com/JimmyKodu/CSharpOfflineAI.git
cd CSharpOfflineAI

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the demo
dotnet run --project src/CSharpOfflineAI.Console/CSharpOfflineAI.Console.csproj
```

### Dependency Injection Setup

```csharp
using CSharpOfflineAI.Core;
using CSharpOfflineAI.MCP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Add services
builder.Services.AddOfflineAI();
builder.Services.AddMCPServer();

// Configure logging
builder.Logging.AddConsole();

var host = builder.Build();
```

## Using Offline AI Service

### Basic Usage

```csharp
using CSharpOfflineAI.Core;

// Get the service from DI
var aiService = host.Services.GetRequiredService<IOfflineAIService>();

// Initialize the service
await aiService.InitializeAsync();

// Check initialization status
if (aiService.IsInitialized)
{
    Console.WriteLine("AI Service is ready!");
}

// Generate a response
var prompt = "Explain machine learning concepts";
var response = await aiService.GenerateResponseAsync(prompt);
Console.WriteLine($"Response: {response}");

// Get model information
var modelInfo = aiService.GetModelInfo();
Console.WriteLine($"Model: {modelInfo}");
```

### Error Handling

```csharp
try
{
    await aiService.InitializeAsync();
    var response = await aiService.GenerateResponseAsync("Your prompt here");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Service not initialized: {ex.Message}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

### Cancellation Support

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    await aiService.InitializeAsync(cts.Token);
    var response = await aiService.GenerateResponseAsync("prompt", cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was cancelled");
}
```

## Creating Custom Agents

### Simple Custom Agent

```csharp
using CSharpOfflineAI.Core;
using Microsoft.Extensions.Logging;

public class TextAnalysisAgent : OfflineAgentBase
{
    public TextAnalysisAgent(
        IOfflineAIService aiService,
        ILogger<TextAnalysisAgent> logger)
        : base(
            "text-analysis-001",
            "Text Analysis Agent",
            "Analyzes and processes text content",
            aiService,
            logger)
    {
    }

    public override async Task<AgentResponse> ExecuteAsync(
        AgentRequest request,
        CancellationToken cancellationToken = default)
    {
        // Add custom pre-processing
        var processedInput = PreprocessText(request.Input);

        // Call base implementation
        var response = await base.ExecuteAsync(
            new AgentRequest(processedInput, request.Context),
            cancellationToken);

        // Add custom post-processing
        if (response.Success)
        {
            var enhancedOutput = PostprocessText(response.Output);
            return response with { Output = enhancedOutput };
        }

        return response;
    }

    private string PreprocessText(string text)
    {
        // Custom preprocessing logic
        return $"[ANALYSIS] {text}";
    }

    private string PostprocessText(string text)
    {
        // Custom postprocessing logic
        return $"{text} [ANALYZED]";
    }
}
```

### Registering Custom Agents

```csharp
// Register in DI container
builder.Services.AddOfflineAgent<TextAnalysisAgent>();

// Or register multiple agents
builder.Services.AddOfflineAgent<TextAnalysisAgent>();
builder.Services.AddOfflineAgent<DataProcessingAgent>();
builder.Services.AddOfflineAgent<CodeGenerationAgent>();
```

### Using Custom Agents

```csharp
var agent = host.Services.GetRequiredService<TextAnalysisAgent>();

var request = new AgentRequest(
    "Analyze this text for sentiment",
    new Dictionary<string, object>
    {
        ["language"] = "en",
        ["detailed"] = true
    }
);

var response = await agent.ExecuteAsync(request);

if (response.Success)
{
    Console.WriteLine($"Agent: {agent.Name}");
    Console.WriteLine($"Result: {response.Output}");

    if (response.Metadata != null)
    {
        foreach (var kvp in response.Metadata)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
    }
}
else
{
    Console.WriteLine($"Error: {response.ErrorMessage}");
}
```

## MCP Server Integration

### Starting the MCP Server

```csharp
using CSharpOfflineAI.MCP;

var mcpServer = host.Services.GetRequiredService<IMCPServer>();

// Start the server
await mcpServer.StartAsync();
Console.WriteLine("MCP Server started");
```

### Processing MCP Messages

#### Initialize Request

```csharp
using System.Text.Json;

var initMessage = new MCPMessage
{
    Id = "init-1",
    Method = "initialize",
    Params = JsonSerializer.SerializeToElement(new
    {
        clientInfo = new
        {
            name = "MyClient",
            version = "1.0.0"
        }
    })
};

var response = await mcpServer.ProcessMessageAsync(initMessage);
Console.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions
{
    WriteIndented = true
}));
```

#### Completion Request

```csharp
var completionMessage = new MCPMessage
{
    Id = "completion-1",
    Method = "completion",
    Params = JsonSerializer.SerializeToElement(new
    {
        prompt = "Generate a hello world program in Python"
    })
};

var response = await mcpServer.ProcessMessageAsync(completionMessage);

if (response.Result.HasValue)
{
    var result = response.Result.Value;
    Console.WriteLine($"Completion: {result.GetProperty("completion").GetString()}");
}
```

#### List Tools

```csharp
var toolsMessage = new MCPMessage
{
    Id = "tools-1",
    Method = "tools/list"
};

var response = await mcpServer.ProcessMessageAsync(toolsMessage);

if (response.Result.HasValue)
{
    var tools = response.Result.Value.GetProperty("tools");
    foreach (var tool in tools.EnumerateArray())
    {
        Console.WriteLine($"Tool: {tool.GetProperty("name").GetString()}");
        Console.WriteLine($"Description: {tool.GetProperty("description").GetString()}");
    }
}
```

#### Ping

```csharp
var pingMessage = new MCPMessage
{
    Id = "ping-1",
    Method = "ping"
};

var response = await mcpServer.ProcessMessageAsync(pingMessage);
Console.WriteLine("Server is alive!");
```

### Stopping the Server

```csharp
await mcpServer.StopAsync();
Console.WriteLine("MCP Server stopped");
```

## Advanced Scenarios

### Multi-Agent Workflow

```csharp
// Create multiple specialized agents
var textAgent = new TextAnalysisAgent(aiService, loggerFactory.CreateLogger<TextAnalysisAgent>());
var codeAgent = new CodeGenerationAgent(aiService, loggerFactory.CreateLogger<CodeGenerationAgent>());
var reviewAgent = new CodeReviewAgent(aiService, loggerFactory.CreateLogger<CodeReviewAgent>());

// Step 1: Analyze requirements
var analysisRequest = new AgentRequest("Create a REST API endpoint");
var analysis = await textAgent.ExecuteAsync(analysisRequest);

// Step 2: Generate code
var codeRequest = new AgentRequest(analysis.Output);
var code = await codeAgent.ExecuteAsync(codeRequest);

// Step 3: Review code
var reviewRequest = new AgentRequest(code.Output);
var review = await reviewAgent.ExecuteAsync(reviewRequest);

Console.WriteLine($"Final output: {review.Output}");
```

### Background Processing

```csharp
// Process multiple requests concurrently
var prompts = new[]
{
    "Explain async programming",
    "What is dependency injection?",
    "Describe SOLID principles"
};

var tasks = prompts.Select(prompt =>
    aiService.GenerateResponseAsync(prompt));

var responses = await Task.WhenAll(tasks);

for (int i = 0; i < prompts.Length; i++)
{
    Console.WriteLine($"Prompt: {prompts[i]}");
    Console.WriteLine($"Response: {responses[i]}");
    Console.WriteLine();
}
```

### MCP Server as HTTP Endpoint (Conceptual)

```csharp
// Note: This is conceptual - you'd need to add ASP.NET Core
// This shows how you might expose MCP over HTTP

app.MapPost("/mcp", async (MCPMessage message, IMCPServer server) =>
{
    try
    {
        var response = await server.ProcessMessageAsync(message);
        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});
```

### Custom AI Service Implementation

```csharp
public class OnnxOfflineAIService : IOfflineAIService
{
    private InferenceSession? _session;

    public bool IsInitialized => _session != null;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // Load ONNX model
        _session = new InferenceSession("path/to/model.onnx");
        await Task.CompletedTask;
    }

    public async Task<string> GenerateResponseAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        if (!IsInitialized)
            throw new InvalidOperationException("Service not initialized");

        // Run inference with ONNX Runtime
        // ... implementation details ...
        
        return await Task.FromResult("Response from ONNX model");
    }

    public string GetModelInfo()
    {
        return IsInitialized
            ? "ONNX Runtime Model"
            : "Model not initialized";
    }
}
```

## Best Practices

1. **Always initialize services before use**
   ```csharp
   if (!aiService.IsInitialized)
   {
       await aiService.InitializeAsync();
   }
   ```

2. **Use cancellation tokens for long-running operations**
   ```csharp
   using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
   await aiService.GenerateResponseAsync(prompt, cts.Token);
   ```

3. **Handle errors gracefully**
   ```csharp
   var response = await agent.ExecuteAsync(request);
   if (!response.Success)
   {
       // Handle error
       logger.LogError(response.ErrorMessage);
   }
   ```

4. **Use dependency injection**
   ```csharp
   // Don't create instances manually
   // DO use DI
   var service = serviceProvider.GetRequiredService<IOfflineAIService>();
   ```

5. **Dispose resources properly**
   ```csharp
   await using var host = builder.Build();
   // Services will be disposed automatically
   ```

## Troubleshooting

### Service Not Initialized
```csharp
// Always check initialization
if (!aiService.IsInitialized)
{
    await aiService.InitializeAsync();
}
```

### MCP Server Errors
```csharp
// Check for error in response
if (response.Error != null)
{
    Console.WriteLine($"Error {response.Error.Code}: {response.Error.Message}");
}
```

### Agent Failures
```csharp
// Check response success
if (!response.Success)
{
    logger.LogError($"Agent failed: {response.ErrorMessage}");
}
```
