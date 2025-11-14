# Contributing to CSharpOfflineAI

Thank you for your interest in contributing to CSharpOfflineAI! This document provides guidelines for contributing to the project.

## Getting Started

1. **Fork the repository** on GitHub
2. **Clone your fork** locally
   ```bash
   git clone https://github.com/YOUR-USERNAME/CSharpOfflineAI.git
   cd CSharpOfflineAI
   ```
3. **Create a branch** for your changes
   ```bash
   git checkout -b feature/my-new-feature
   ```

## Development Setup

### Prerequisites

- .NET 9.0 SDK or later
- A code editor (Visual Studio, VS Code, or Rider recommended)

### Building the Project

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Running the Demo

```bash
dotnet run --project src/CSharpOfflineAI.Console/CSharpOfflineAI.Console.csproj
```

## Code Style

### General Guidelines

- Follow [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
- Use meaningful variable and method names
- Keep methods focused and concise
- Add XML documentation comments for public APIs

### Formatting

- Use 4 spaces for indentation (no tabs)
- Place opening braces on new lines
- Use `var` for local variables when the type is obvious
- Use null-coalescing operators when appropriate

Example:
```csharp
public async Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default)
{
    if (string.IsNullOrWhiteSpace(prompt))
    {
        throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));
    }

    var result = await ProcessPromptAsync(prompt, cancellationToken);
    return result ?? string.Empty;
}
```

## Testing

### Writing Tests

- Write unit tests for all new functionality
- Use xUnit as the testing framework
- Follow the Arrange-Act-Assert pattern
- Use meaningful test names that describe the scenario

Example:
```csharp
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
}
```

### Test Coverage

- Aim for high test coverage (>80%)
- Test both success and failure scenarios
- Test edge cases and boundary conditions

## Submitting Changes

### Commit Messages

Use clear and descriptive commit messages:

- Start with a verb in present tense (Add, Fix, Update, Remove)
- Keep the first line under 72 characters
- Add a detailed description if needed

Good examples:
```
Add support for custom AI models

Fix initialization error in OfflineMCPServer

Update documentation for agent framework
```

### Pull Request Process

1. **Update documentation** if you've added or changed functionality
2. **Add tests** for new features
3. **Run all tests** to ensure nothing is broken
4. **Update the README** if needed
5. **Create a pull request** with a clear description

Pull request template:
```markdown
## Description
Brief description of the changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
Describe the testing you've done

## Checklist
- [ ] Code follows the style guidelines
- [ ] Self-review completed
- [ ] Tests added/updated
- [ ] Documentation updated
- [ ] All tests pass
```

## Feature Requests and Bug Reports

### Reporting Bugs

When reporting bugs, please include:

1. **Description** - Clear description of the issue
2. **Steps to Reproduce** - Detailed steps to reproduce the problem
3. **Expected Behavior** - What you expected to happen
4. **Actual Behavior** - What actually happened
5. **Environment** - OS, .NET version, etc.

Example:
```markdown
**Description**
AI service fails to initialize when...

**Steps to Reproduce**
1. Create a new instance of LocalOfflineAIService
2. Call InitializeAsync()
3. Observe the error

**Expected Behavior**
Service should initialize successfully

**Actual Behavior**
Throws InvalidOperationException

**Environment**
- OS: Windows 11
- .NET: 9.0
- Version: 1.0.0
```

### Suggesting Features

When suggesting features, please include:

1. **Use Case** - Why is this feature needed?
2. **Proposed Solution** - How should it work?
3. **Alternatives** - What alternatives have you considered?
4. **Additional Context** - Any other relevant information

## Code Review Process

All contributions go through code review:

1. **Automated checks** must pass (build, tests)
2. **At least one maintainer** must approve
3. **Address feedback** from reviewers
4. **Squash commits** if requested

## Project Structure

```
CSharpOfflineAI/
├── src/
│   ├── CSharpOfflineAI.Core/       # Core AI services and agents
│   ├── CSharpOfflineAI.MCP/        # MCP implementation
│   └── CSharpOfflineAI.Console/    # Demo application
├── tests/
│   └── CSharpOfflineAI.Core.Tests/ # Unit tests
├── README.md                        # Project overview
├── ARCHITECTURE.md                  # Architecture documentation
├── EXAMPLES.md                      # Usage examples
└── CONTRIBUTING.md                  # This file
```

## Areas for Contribution

### High Priority

- Integration with real local AI models (ONNX, llama.cpp)
- Additional MCP methods and capabilities
- Performance optimizations
- More comprehensive examples

### Medium Priority

- Additional agent types
- Model management utilities
- Caching improvements
- Enhanced logging

### Low Priority

- UI components
- Additional documentation
- Code cleanup and refactoring

## Questions?

If you have questions:

1. Check the [README.md](README.md)
2. Review [ARCHITECTURE.md](ARCHITECTURE.md)
3. Look at [EXAMPLES.md](EXAMPLES.md)
4. Open a GitHub issue for discussion

## License

By contributing to CSharpOfflineAI, you agree that your contributions will be licensed under the same license as the project.

## Recognition

Contributors will be recognized in:
- Project README
- Release notes
- GitHub contributors page

Thank you for contributing to CSharpOfflineAI! 🎉
