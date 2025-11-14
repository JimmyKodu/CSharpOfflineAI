using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSharpOfflineAI.MCP;

/// <summary>
/// Represents a Model Context Protocol (MCP) message
/// </summary>
public class MCPMessage
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("params")]
    public JsonElement? Params { get; set; }

    [JsonPropertyName("result")]
    public JsonElement? Result { get; set; }

    [JsonPropertyName("error")]
    public MCPError? Error { get; set; }
}

/// <summary>
/// Represents an error in MCP communication
/// </summary>
public class MCPError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public JsonElement? Data { get; set; }
}
