using CSharpOfflineAI.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CSharpOfflineAI.Core.Tests;

public class OfflineAgentBaseTests
{
    private readonly Mock<IOfflineAIService> _aiServiceMock;
    private readonly Mock<ILogger<OfflineAgentBase>> _loggerMock;
    private readonly OfflineAgentBase _agent;

    public OfflineAgentBaseTests()
    {
        _aiServiceMock = new Mock<IOfflineAIService>();
        _loggerMock = new Mock<ILogger<OfflineAgentBase>>();
        _agent = new OfflineAgentBase(
            "test-id",
            "Test Agent",
            "Test Description",
            _aiServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public void Constructor_WithNullId_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OfflineAgentBase(
            null!,
            "name",
            "description",
            _aiServiceMock.Object,
            _loggerMock.Object
        ));
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OfflineAgentBase(
            "id",
            null!,
            "description",
            _aiServiceMock.Object,
            _loggerMock.Object
        ));
    }

    [Fact]
    public void Constructor_WithNullDescription_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OfflineAgentBase(
            "id",
            "name",
            null!,
            _aiServiceMock.Object,
            _loggerMock.Object
        ));
    }

    [Fact]
    public void Constructor_WithNullAIService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OfflineAgentBase(
            "id",
            "name",
            "description",
            null!,
            _loggerMock.Object
        ));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OfflineAgentBase(
            "id",
            "name",
            "description",
            _aiServiceMock.Object,
            null!
        ));
    }

    [Fact]
    public void Properties_ReturnCorrectValues()
    {
        // Assert
        Assert.Equal("test-id", _agent.Id);
        Assert.Equal("Test Agent", _agent.Name);
        Assert.Equal("Test Description", _agent.Description);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _agent.ExecuteAsync(null!));
    }

    [Fact]
    public async Task ExecuteAsync_InitializesAIService_WhenNotInitialized()
    {
        // Arrange
        _aiServiceMock.Setup(x => x.IsInitialized).Returns(false);
        _aiServiceMock.Setup(x => x.InitializeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _aiServiceMock.Setup(x => x.GenerateResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("test response");
        _aiServiceMock.Setup(x => x.GetModelInfo()).Returns("model info");

        var request = new AgentRequest("test input");

        // Act
        await _agent.ExecuteAsync(request);

        // Assert
        _aiServiceMock.Verify(x => x.InitializeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        _aiServiceMock.Setup(x => x.IsInitialized).Returns(true);
        _aiServiceMock.Setup(x => x.GenerateResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("test response");
        _aiServiceMock.Setup(x => x.GetModelInfo()).Returns("model info");

        var request = new AgentRequest("test input");

        // Act
        var response = await _agent.ExecuteAsync(request);

        // Assert
        Assert.True(response.Success);
        Assert.Equal("test response", response.Output);
        Assert.Null(response.ErrorMessage);
        Assert.NotNull(response.Metadata);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_IncludesMetadata()
    {
        // Arrange
        _aiServiceMock.Setup(x => x.IsInitialized).Returns(true);
        _aiServiceMock.Setup(x => x.GenerateResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("test response");
        _aiServiceMock.Setup(x => x.GetModelInfo()).Returns("model info");

        var request = new AgentRequest("test input");

        // Act
        var response = await _agent.ExecuteAsync(request);

        // Assert
        Assert.NotNull(response.Metadata);
        Assert.Equal("test-id", response.Metadata["agent_id"]);
        Assert.Equal("Test Agent", response.Metadata["agent_name"]);
        Assert.Equal("model info", response.Metadata["model_info"]);
        Assert.Contains("timestamp", response.Metadata.Keys);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAIServiceThrows_ReturnsFailureResponse()
    {
        // Arrange
        _aiServiceMock.Setup(x => x.IsInitialized).Returns(true);
        _aiServiceMock.Setup(x => x.GenerateResponseAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test error"));

        var request = new AgentRequest("test input");

        // Act
        var response = await _agent.ExecuteAsync(request);

        // Assert
        Assert.False(response.Success);
        Assert.Equal(string.Empty, response.Output);
        Assert.Equal("Test error", response.ErrorMessage);
    }
}
