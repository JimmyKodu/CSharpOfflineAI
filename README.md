# CSharpOfflineAI

A comprehensive C# solution for running AI applications offline using **Microsoft.Extensions.AI**, **Microsoft Agent Framework** concepts, and **Model Context Protocol (MCP)**.

## Overview

This project demonstrates how to build AI-powered applications that work completely offline, without requiring internet connectivity. It integrates:

- **Microsoft.Extensions.AI** - Microsoft's AI abstraction library
- **Microsoft Agent Framework principles** - Agent-based AI architecture
- **Model Context Protocol (MCP)** - Standardized protocol for AI model communication

## Features

✅ **Offline AI Service** - Run AI models locally without internet
✅ **Agent Framework** - Build intelligent agents that execute tasks independently
✅ **MCP Server** - Implement Model Context Protocol for standardized AI communication
✅ **Dependency Injection** - Full integration with Microsoft.Extensions.DependencyInjection
✅ **Logging Support** - Built-in logging with Microsoft.Extensions.Logging
✅ **No Internet Required** - All operations work completely offline

## Project Structure

```
CSharpOfflineAI/
├── src/
│   ├── CSharpOfflineAI.Core/          # Core offline AI services and agent framework
│   ├── CSharpOfflineAI.MCP/           # Model Context Protocol implementation
│   └── CSharpOfflineAI.Console/       # Demo console application
└── README.md
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run --project src/CSharpOfflineAI.Console/CSharpOfflineAI.Console.csproj
```

## Usage Examples

### 1. Using Offline AI Service

```csharp
using CSharpOfflineAI.Core;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddOfflineAI();
services.AddLogging();

var serviceProvider = services.BuildServiceProvider();
var aiService = serviceProvider.GetRequiredService<IOfflineAIService>();

await aiService.InitializeAsync();
var response = await aiService.GenerateResponseAsync("Your prompt here");
Console.WriteLine(response);
```

### 2. Creating Offline Agents

```csharp
using CSharpOfflineAI.Core;

var agent = new OfflineAgentBase(
    id: "agent-001",
    name: "My Assistant",
    description: "An offline AI assistant",
    aiService: aiService,
    logger: logger
);

var request = new AgentRequest("Help me with a task");
var response = await agent.ExecuteAsync(request);

if (response.Success)
{
    Console.WriteLine($"Agent response: {response.Output}");
}
```

### 3. Using Model Context Protocol

```csharp
using CSharpOfflineAI.MCP;
using System.Text.Json;

var services = new ServiceCollection();
services.AddOfflineAI();
services.AddMCPServer();

var mcpServer = serviceProvider.GetRequiredService<IMCPServer>();
await mcpServer.StartAsync();

// Send MCP message
var message = new MCPMessage
{
    Id = "1",
    Method = "completion",
    Params = JsonSerializer.SerializeToElement(new { prompt = "Your prompt" })
};

var response = await mcpServer.ProcessMessageAsync(message);
```

## Key Components

### IOfflineAIService

Core interface for offline AI operations:
- `InitializeAsync()` - Initialize the AI service with local models
- `GenerateResponseAsync()` - Generate responses using local models
- `GetModelInfo()` - Get information about the loaded model

### IOfflineAgent

Agent interface following Microsoft Agent Framework principles:
- `ExecuteAsync()` - Execute agent tasks
- Agent metadata (Id, Name, Description)
- Context-aware request/response handling

### IMCPServer

Model Context Protocol server:
- `StartAsync()` / `StopAsync()` - Manage server lifecycle
- `ProcessMessageAsync()` - Handle MCP messages
- JSON-RPC 2.0 compatible
- Supports multiple MCP methods (initialize, completion, tools/list, ping)

## Architecture

### Offline Operation

The solution is designed to work entirely offline by:
1. Using local AI models (configured during initialization)
2. No external API calls or internet dependencies
3. Self-contained processing pipeline
4. Local model caching and management

### Microsoft.Extensions.AI Integration

Leverages Microsoft's AI abstractions for:
- Standardized AI service interfaces
- Seamless integration with .NET ecosystem
- Future compatibility with Microsoft AI services

### Agent Framework Design

Implements agent-based architecture:
- Autonomous agents with specific capabilities
- Request/response pattern with context
- Metadata tracking and logging
- Extensible agent base classes

### Model Context Protocol

Full MCP implementation:
- JSON-RPC 2.0 protocol
- Standard MCP methods support
- Tool discovery and execution
- Error handling and responses

## Extensibility

### Custom Agents

Create custom agents by inheriting from `OfflineAgentBase`:

```csharp
public class MyCustomAgent : OfflineAgentBase
{
    public MyCustomAgent(IOfflineAIService aiService, ILogger<MyCustomAgent> logger)
        : base("custom-001", "Custom Agent", "My custom agent", aiService, logger)
    {
    }

    public override async Task<AgentResponse> ExecuteAsync(
        AgentRequest request, 
        CancellationToken cancellationToken = default)
    {
        // Custom implementation
        return await base.ExecuteAsync(request, cancellationToken);
    }
}
```

### Custom AI Services

Implement `IOfflineAIService` for custom AI backends:

```csharp
public class MyCustomAIService : IOfflineAIService
{
    // Implement interface methods for your specific AI model
}
```

## Configuration

Services can be configured using dependency injection:

```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOfflineAI();
builder.Services.AddMCPServer();
builder.Services.AddOfflineAgent<MyCustomAgent>();

var host = builder.Build();
```

## Technologies

- **.NET 9.0** - Target framework
- **Microsoft.Extensions.AI** - AI abstractions (Preview)
- **Microsoft.Extensions.DependencyInjection** - DI container
- **Microsoft.Extensions.Logging** - Logging infrastructure
- **Microsoft.Extensions.Hosting** - Application hosting
- **System.Text.Json** - JSON serialization

## License

This project is provided as-is for educational and demonstration purposes.

## Contributing

This is a demonstration project showing offline AI capabilities. Feel free to use it as a starting point for your own offline AI applications.

## Notes

- This implementation uses simulated responses for demonstration. In production, integrate with actual local AI models.
- Local AI models should be downloaded and configured before initialization.
- The solution is designed to work without any internet connectivity once models are installed.

## Future Enhancements

- Integration with local LLM models (e.g., ONNX, llama.cpp)
- Model management and versioning
- Advanced agent orchestration
- MCP extensions for specialized domains
- Caching and performance optimizations