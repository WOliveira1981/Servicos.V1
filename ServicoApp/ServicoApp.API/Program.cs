using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using ServicoApp.Application;
using ServicoApp.Application.Auth;
using ServicoApp.Application.Services;
using ServicoApp.Infrastructure;
using ServicoApp.Infrastructure.Persistence;

Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<SqliteDatabaseInitializer>();
    await initializer.InitializeAsync();
}

app.MapGet("/servicos", async (IServicoAppService service) =>
{
    var servicos = await service.GetAllAsync();
    return Results.Ok(servicos);
})
.WithName("GetServicos")
.WithOpenApi();

app.MapGet("/servicos/{id:guid}", async (Guid id, IServicoAppService service) =>
{
    var servico = await service.GetByIdAsync(id);
    return servico is null
        ? Results.NotFound()
        : Results.Ok(servico);
})
.WithName("GetServicoById")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/servicos", async (CreateServicoRequest request, IServicoAppService service) =>
{
    var servico = await service.CreateAsync(request.Nome, request.Descricao, request.Valor);
    return Results.Created($"/servicos/{servico.Id}", servico);
})
.WithName("CreateServico")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/auth/login", async (LoginRequest request, AuthenticationService authService) =>
{
    var result = await authService.AuthenticateAsync(
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
.WithName("Login")
.WithOpenApi();

app.Run();

public record CreateServicoRequest(string Nome, string Descricao, decimal Valor);
public record LoginRequest(string Provider, string? Token, string? Email, string? Name = null);
