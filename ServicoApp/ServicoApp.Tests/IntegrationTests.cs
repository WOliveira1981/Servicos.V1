using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace ServicoApp.Tests;

/// <summary>
/// Testes de integração para verificar o comportamento real da API pública.
/// </summary>
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Padrão Test Double: o WebApplicationFactory simula a aplicação real para validar
        // o comportamento HTTP sem precisar subir o servidor manualmente.
        _factory = factory;
    }

    [Fact]
    public async Task SwaggerEndpoint_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GatewayLoginEndpoint_ReturnsTokenForGoogleProvider()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/gateway/auth/login",
            new { Provider = "google", Email = "user@example.com" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<GatewayLoginResponse>();
        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload!.JwtToken));
    }

    private sealed record GatewayLoginResponse(string JwtToken, string? Provider, string? Email);
}