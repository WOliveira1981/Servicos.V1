using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using ServicoApp.Application;
using ServicoApp.Application.Auth;
using ServicoApp.Application.Gateway;
using ServicoApp.Application.Services;
using ServicoApp.Infrastructure;
using ServicoApp.Infrastructure.Persistence;

Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://frontend:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? "super-secret-key-for-dev-only-change-me";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ServicoApp";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ServicoAppClients";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Padrão Dependency Injection (Inversão de Controle): registra as dependências da aplicação.
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IAuthenticationStrategy, GoogleOAuthStrategy>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<AuthenticationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<SqliteDatabaseInitializer>();
    await initializer.InitializeAsync();
}

// Padrão Facade: o gateway expõe um único ponto de entrada para múltiplos fluxos da aplicação.
app.MapGet("/gateway/servicos", async (IApiGatewayService gateway) =>
{
    var servicos = await gateway.GetAllAsync();
    return Results.Ok(servicos);
})
.WithName("GetServicosViaGateway")
.WithOpenApi();

app.MapGet("/gateway/servicos/{id:guid}", async (Guid id, IApiGatewayService gateway) =>
{
    var servico = await gateway.GetByIdAsync(id);
    return servico is null
        ? Results.NotFound()
        : Results.Ok(servico);
})
.WithName("GetServicoByIdViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/gateway/servicos", async (CreateServicoRequest request, IApiGatewayService gateway) =>
{
    var servico = await gateway.CreateAsync(request.Nome, request.Descricao, request.Valor);
    return Results.Created($"/gateway/servicos/{servico.Id}", servico);
})
.WithName("CreateServicoViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapPatch("/gateway/servicos/{id:guid}/status", async (Guid id, UpdateStatusRequest request, IApiGatewayService gateway) =>
{
    var servico = await gateway.AlterarStatusAsync(id, request.Ativo);
    return servico is null
        ? Results.NotFound()
        : Results.Ok(servico);
})
.WithName("UpdateServicoStatusViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapGet("/gateway/orcamentos", async (IApiGatewayService gateway) =>
{
    var orcamentos = await gateway.GetOrcamentosAsync();
    return Results.Ok(orcamentos);
})
.WithName("GetOrcamentosViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/gateway/orcamentos", async (CreateOrcamentoRequest request, IApiGatewayService gateway) =>
{
    try
    {
        var orcamento = await gateway.CriarOrcamentoAsync(request.ServicoId, request.ValorTotal);
        return Results.Created($"/gateway/orcamentos/{orcamento.Id}", orcamento);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
})
.WithName("CreateOrcamentoViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapGet("/gateway/agenda", async (IApiGatewayService gateway) =>
{
    var agenda = await gateway.GetAgendaAsync();
    return Results.Ok(agenda);
})
.WithName("GetAgendaViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapGet("/gateway/historico", async (IApiGatewayService gateway) =>
{
    var historico = await gateway.GetHistoricoAsync();
    return Results.Ok(historico);
})
.WithName("GetHistoricoViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/gateway/auth/login", async (LoginRequest request, IApiGatewayService gateway) =>
{
    var result = await gateway.LoginAsync(
        new AuthenticationRequest(request.Provider, request.Token, request.Email, request.Name));

    if (!result.IsAuthenticated || string.IsNullOrWhiteSpace(result.JwtToken))
    {
        return Results.BadRequest(new { result.Message });
    }

    return Results.Ok(new
    {
        result.JwtToken,
        result.Provider,
        result.Email
    });
})
.WithName("LoginViaGateway")
.WithOpenApi();

app.Run();

public partial class Program;

public record CreateServicoRequest(string Nome, string Descricao, decimal Valor);
public record CreateOrcamentoRequest(Guid ServicoId, decimal ValorTotal);
public record UpdateStatusRequest(bool Ativo);
public record LoginRequest(string Provider, string? Token, string? Email, string? Name = null);
