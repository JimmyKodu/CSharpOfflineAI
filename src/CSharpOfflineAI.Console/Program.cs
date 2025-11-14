using CSharpOfflineAI.Core;
using CSharpOfflineAI.MCP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

Console.WriteLine("=== C# Offline AI Demo ===");
Console.WriteLine("Demonstrating offline AI with Microsoft.Extensions.AI, Agent Framework, and Model Context Protocol");
Console.WriteLine();

// Build the host with dependency injection
var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Register offline AI services
builder.Services.AddOfflineAI();
builder.Services.AddMCPServer();

var host = builder.Build();

// Get services
var aiService = host.Services.GetRequiredService<IOfflineAIService>();
var mcpServer = host.Services.GetRequiredService<IMCPServer>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    // 1. Demonstrate Offline AI Service
    Console.WriteLine("--- 1. Offline AI Service Demo ---");
    await aiService.InitializeAsync();
    Console.WriteLine($"Model Info: {aiService.GetModelInfo()}");
    Console.WriteLine();

    var testPrompt = "What is the purpose of offline AI?";
    Console.WriteLine($"Prompt: {testPrompt}");
    var response = await aiService.GenerateResponseAsync(testPrompt);
    Console.WriteLine($"Response: {response}");
    Console.WriteLine();

    // 2. Demonstrate Agent Framework
    Console.WriteLine("--- 2. Agent Framework Demo ---");
    var agent = new OfflineAgentBase(
        "agent-001",
        "Assistant Agent",
        "An offline AI assistant agent",
        aiService,
        host.Services.GetRequiredService<ILogger<OfflineAgentBase>>()
    );

    var agentRequest = new AgentRequest("Help me understand offline AI capabilities");
    var agentResponse = await agent.ExecuteAsync(agentRequest);

    Console.WriteLine($"Agent: {agent.Name}");
    Console.WriteLine($"Success: {agentResponse.Success}");
    Console.WriteLine($"Output: {agentResponse.Output}");
    if (agentResponse.Metadata != null)
    {
        Console.WriteLine($"Metadata: {JsonSerializer.Serialize(agentResponse.Metadata, new JsonSerializerOptions { WriteIndented = true })}");
    }
    Console.WriteLine();

    // 3. Demonstrate Model Context Protocol
    Console.WriteLine("--- 3. Model Context Protocol (MCP) Demo ---");
    await mcpServer.StartAsync();

    // Test initialize
    var initMessage = new MCPMessage
    {
        Id = "1",
        Method = "initialize",
        Params = JsonSerializer.SerializeToElement(new { })
    };
    var initResponse = await mcpServer.ProcessMessageAsync(initMessage);
    Console.WriteLine($"MCP Initialize Response: {JsonSerializer.Serialize(initResponse, new JsonSerializerOptions { WriteIndented = true })}");
    Console.WriteLine();

    // Test completion
    var completionMessage = new MCPMessage
    {
        Id = "2",
        Method = "completion",
        Params = JsonSerializer.SerializeToElement(new { prompt = "Explain offline AI processing" })
    };
    var completionResponse = await mcpServer.ProcessMessageAsync(completionMessage);
    Console.WriteLine($"MCP Completion Response: {JsonSerializer.Serialize(completionResponse, new JsonSerializerOptions { WriteIndented = true })}");
    Console.WriteLine();

    // Test tools list
    var toolsMessage = new MCPMessage
    {
        Id = "3",
        Method = "tools/list"
    };
    var toolsResponse = await mcpServer.ProcessMessageAsync(toolsMessage);
    Console.WriteLine($"MCP Tools Response: {JsonSerializer.Serialize(toolsResponse, new JsonSerializerOptions { WriteIndented = true })}");
    Console.WriteLine();

    await mcpServer.StopAsync();

    Console.WriteLine("--- Demo Completed Successfully ---");
    Console.WriteLine("All components are working offline without internet connectivity!");
}
catch (Exception ex)
{
    logger.LogError(ex, "Error running demo");
    Console.WriteLine($"Error: {ex.Message}");
    return 1;
}

return 0;
