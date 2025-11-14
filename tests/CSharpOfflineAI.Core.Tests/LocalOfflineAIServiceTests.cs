using CSharpOfflineAI.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CSharpOfflineAI.Core.Tests;

public class LocalOfflineAIServiceTests
{
    private readonly Mock<ILogger<LocalOfflineAIService>> _loggerMock;
    private readonly LocalOfflineAIService _service;

    public LocalOfflineAIServiceTests()
    {
        _loggerMock = new Mock<ILogger<LocalOfflineAIService>>();
        _service = new LocalOfflineAIService(_loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LocalOfflineAIService(null!));
    }

    [Fact]
    public void IsInitialized_BeforeInitialization_ReturnsFalse()
    {
        // Assert
        Assert.False(_service.IsInitialized);
    }

    [Fact]
    public async Task InitializeAsync_SuccessfullyInitializes()
    {
        // Act
        await _service.InitializeAsync();

        // Assert
        Assert.True(_service.IsInitialized);
    }

    [Fact]
    public async Task GenerateResponseAsync_BeforeInitialization_ThrowsInvalidOperationException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.GenerateResponseAsync("test prompt"));
    }

    [Fact]
    public async Task GenerateResponseAsync_WithNullPrompt_ThrowsArgumentException()
    {
        // Arrange
        await _service.InitializeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GenerateResponseAsync(null!));
    }

    [Fact]
    public async Task GenerateResponseAsync_WithEmptyPrompt_ThrowsArgumentException()
    {
        // Arrange
        await _service.InitializeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GenerateResponseAsync(string.Empty));
    }

    [Fact]
    public async Task GenerateResponseAsync_WithValidPrompt_ReturnsResponse()
    {
        // Arrange
        await _service.InitializeAsync();
        var prompt = "test prompt";

        // Act
        var response = await _service.GenerateResponseAsync(prompt);

        // Assert
        Assert.NotNull(response);
        Assert.Contains(prompt, response);
        Assert.Contains("Offline AI Response", response);
    }

    [Fact]
    public void GetModelInfo_BeforeInitialization_ReturnsNotInitializedMessage()
    {
        // Act
        var info = _service.GetModelInfo();

        // Assert
        Assert.Equal("Model not initialized", info);
    }

    [Fact]
    public async Task GetModelInfo_AfterInitialization_ReturnsModelInfo()
    {
        // Arrange
        await _service.InitializeAsync();

        // Act
        var info = _service.GetModelInfo();

        // Assert
        Assert.Contains("Local Offline Model", info);
        Assert.Contains("Ready", info);
        Assert.Contains("Offline", info);
    }

    [Fact]
    public async Task GenerateResponseAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        // Arrange
        await _service.InitializeAsync();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => 
            _service.GenerateResponseAsync("test", cts.Token));
    }
}
