# Architecture Overview

## Design Principles

The CSharpOfflineAI solution follows these key principles:

1. **Offline-First**: All operations work without internet connectivity
2. **Modular Design**: Clear separation of concerns across projects
3. **Microsoft Standards**: Uses Microsoft.Extensions.* patterns
4. **Extensibility**: Easy to extend with custom implementations

## Component Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    CSharpOfflineAI.Console                  │
│                    (Demo Application)                        │
└───────────────┬─────────────────────────┬───────────────────┘
                │                         │
                ▼                         ▼
┌──────────────────────────────┐  ┌──────────────────────────┐
│   CSharpOfflineAI.Core       │  │  CSharpOfflineAI.MCP     │
│   - IOfflineAIService        │  │  - IMCPServer            │
│   - LocalOfflineAIService    │  │  - OfflineMCPServer      │
│   - IOfflineAgent            │  │  - MCPMessage            │
│   - OfflineAgentBase         │  │  - MCPError              │
│   - ServiceExtensions        │  │  - ServiceExtensions     │
└──────────────────────────────┘  └──────────────────────────┘
                │                         │
                └────────────┬────────────┘
                             ▼
                ┌────────────────────────┐
                │ Microsoft.Extensions.* │
                │ - AI                   │
                │ - DependencyInjection  │
                │ - Logging              │
                │ - Hosting              │
                └────────────────────────┘
```

## Core Components

### CSharpOfflineAI.Core

The core library provides:

- **IOfflineAIService**: Interface for offline AI operations
- **LocalOfflineAIService**: Implementation using local models
- **IOfflineAgent**: Agent abstraction following Microsoft Agent Framework
- **OfflineAgentBase**: Base class for building agents
- **ServiceCollectionExtensions**: DI registration helpers

### CSharpOfflineAI.MCP

The MCP library implements:

- **IMCPServer**: Model Context Protocol server interface
- **OfflineMCPServer**: Offline MCP server implementation
- **MCPMessage**: JSON-RPC 2.0 message structure
- **MCPError**: Error handling structure
- **MCPServiceCollectionExtensions**: DI registration

### CSharpOfflineAI.Console

Demo application showcasing:

- Offline AI service usage
- Agent framework implementation
- MCP protocol communication
- Full integration example

## Data Flow

### AI Service Request Flow

```
User Input
    │
    ▼
IOfflineAIService.GenerateResponseAsync()
    │
    ▼
LocalOfflineAIService (Process locally)
    │
    ▼
Local AI Model Processing
    │
    ▼
Response Generated
    │
    ▼
Return to User
```

### Agent Execution Flow

```
AgentRequest
    │
    ▼
IOfflineAgent.ExecuteAsync()
    │
    ▼
OfflineAgentBase
    │
    ├─► Check AI Service Initialization
    │
    ├─► Process Request Context
    │
    ├─► Call IOfflineAIService
    │
    ├─► Add Metadata
    │
    ▼
AgentResponse (with Success, Output, Metadata)
```

### MCP Message Flow

```
Client Request (MCPMessage)
    │
    ▼
IMCPServer.ProcessMessageAsync()
    │
    ├─► Parse JSON-RPC Message
    │
    ├─► Route by Method
    │   ├─► initialize
    │   ├─► completion
    │   ├─► tools/list
    │   └─► ping
    │
    ├─► Execute Handler
    │
    ├─► Generate Response
    │
    ▼
MCPMessage Response
```

## Dependency Injection

The solution uses Microsoft.Extensions.DependencyInjection:

```csharp
services.AddOfflineAI();      // Registers IOfflineAIService
services.AddMCPServer();      // Registers IMCPServer
services.AddOfflineAgent<T>(); // Registers custom agents
```

## Extensibility Points

### 1. Custom AI Services

Implement `IOfflineAIService` for different AI backends:

```csharp
public class CustomAIService : IOfflineAIService
{
    // Use ONNX Runtime, llama.cpp, etc.
}
```

### 2. Custom Agents

Extend `OfflineAgentBase` for specialized agents:

```csharp
public class DataAnalysisAgent : OfflineAgentBase
{
    // Custom data analysis logic
}
```

### 3. Custom MCP Handlers

Extend `OfflineMCPServer` to add new methods:

```csharp
public class ExtendedMCPServer : OfflineMCPServer
{
    // Add custom MCP methods
}
```

## Technology Stack

- **.NET 9.0**: Modern .NET features
- **Microsoft.Extensions.AI**: AI abstractions (Preview)
- **Microsoft.Extensions.Hosting**: Application lifetime management
- **Microsoft.Extensions.Logging**: Structured logging
- **System.Text.Json**: High-performance JSON

## Security Considerations

1. **No External Calls**: All processing happens locally
2. **Data Privacy**: No data leaves the local machine
3. **Model Safety**: Use trusted, verified local models
4. **Input Validation**: All inputs are validated before processing

## Performance Characteristics

- **Startup Time**: Fast initialization with local models
- **Response Time**: Depends on local model complexity
- **Memory Usage**: Determined by loaded model size
- **Scalability**: Limited by local hardware resources

## Future Architecture Enhancements

1. **Model Caching**: Improve startup performance
2. **Async Streaming**: Support streaming responses
3. **Multi-Model Support**: Load multiple models simultaneously
4. **Agent Orchestration**: Complex multi-agent workflows
5. **Model Quantization**: Optimize model size and performance
