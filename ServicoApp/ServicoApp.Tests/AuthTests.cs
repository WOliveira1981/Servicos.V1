using Microsoft.Extensions.Logging;
using ServicoApp.Application.Auth;

namespace ServicoApp.Tests;

/// <summary>
/// Testes unitários para autenticação.
/// </summary>
public class AuthTests
{
    [Fact]
    public async Task AuthenticateAsync_WithGoogleProvider_ReturnsSuccess()
    {
        // Arrange
        // Padrão Mock / Test Double: usamos um logger fake apenas para observar o comportamento
        // da estratégia sem depender de infraestrutura externa real.
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var strategy = new GoogleOAuthStrategy(loggerFactory.CreateLogger<GoogleOAuthStrategy>());
        var request = new AuthenticationRequest(
            Provider: "google",
            Token: "fake-token",
            Email: "user@example.com");

        // Act
        var result = await strategy.AuthenticateAsync(request);

        // Assert
        Assert.True(result.IsAuthenticated);
        Assert.Equal("google", result.Provider);
        Assert.Equal("user@example.com", result.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.JwtToken));
    }

    [Fact]
    public async Task AuthenticateAsync_WithUnsupportedProvider_ReturnsFailure()
    {
        // Arrange
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var strategy = new GoogleOAuthStrategy(loggerFactory.CreateLogger<GoogleOAuthStrategy>());
        var request = new AuthenticationRequest(
            Provider: "github",
            Token: "token",
            Email: "user@example.com");

        // Act
        var result = await strategy.AuthenticateAsync(request);

        // Assert
        Assert.False(result.IsAuthenticated);
        Assert.Contains("Provider não suportado", result.Message);
    }
}