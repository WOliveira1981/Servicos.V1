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
using Microsoft.Extensions.Logging;


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
// O tipo genérico ILogger<Program> é apenas uma forma de categorizar os logs com o nome da classe "Program".
// O GUID operationId é gerado para cada requisição e ajuda a rastrear logs relacionados a uma operação específica.
app.MapGet("/gateway/servicos", async (IApiGatewayService gateway, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Listando servicos pelo gateway.");
        var servicos = await gateway.GetAllAsync();
        return Results.Ok(servicos);
    }
    catch (Exception ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogError(ex, "Erro ao listar servicos pelo gateway.operationId={operationId}", operationId);
        return Results.Problem("Ocorreu um erro ao listar os servicos.");
    }
})
.WithName("GetServicosViaGateway")
.WithOpenApi();

app.MapGet("/gateway/servicos/{id:guid}", async (Guid id, IApiGatewayService gateway, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Buscando servico {ServicoId} pelo gateway.", id);
        var servico = await gateway.GetByIdAsync(id);

        if (servico is null)
        {
            var operationId = Guid.NewGuid();
            logger.LogWarning("Servico {ServicoId} nao encontrado. operationId={operationId}", id, operationId);
            return Results.NotFound(new { message = "Servico nao encontrado." });
        }

        return Results.Ok(servico);
    }
    catch (Exception ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogError(ex, "Erro ao buscar servico {ServicoId} pelo gateway. operationId={operationId}", id, operationId);
        return Results.Problem("Ocorreu um erro ao buscar o servico.");
    }
})
.WithName("GetServicoByIdViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/gateway/servicos", async (CreateServicoRequest? request, IApiGatewayService gateway, ILogger<Program> logger) =>
{
    var validationMessage = ValidateCreateServicoRequest(request);
    if (validationMessage is not null)
    {
        var operationId = Guid.NewGuid();
        logger.LogWarning("Requisicao invalida para criar servico: {ValidationMessage}. OperationID={operationId}", validationMessage, operationId);
        return Results.BadRequest(new { message = validationMessage });
    }

    try
    {
        logger.LogInformation("Criando servico {ServicoNome} pelo gateway.", request!.Nome);
        var servico = await gateway.CreateAsync(request.Nome, request.Descricao, request.Valor);
        logger.LogInformation("Servico {ServicoId} criado com sucesso.", servico.Id);
        return Results.Created($"/gateway/servicos/{servico.Id}", servico);
    }
    catch (ArgumentException ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogWarning(ex, "Dados invalidos ao criar servico. OperationID={operationId}", operationId);
        return Results.BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogError(ex, "Erro ao criar servico pelo gateway. OperationID={operationId}", operationId);
        return Results.Problem("Ocorreu um erro ao criar o servico.");
    }
})
.WithName("CreateServicoViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapPatch("/gateway/servicos/{id:guid}/status", async (Guid id, UpdateStatusRequest? request, IApiGatewayService gateway, ILogger<Program> logger) =>
{
    if (request is null)
    {
        var operationId = Guid.NewGuid();
        logger.LogWarning("Requisicao invalida para alterar status do servico {ServicoId}: corpo obrigatorio. OperationID={operationId}", id, operationId);
        return Results.BadRequest(new { message = "Informe os dados para alterar o status do servico." });
    }

    try
    {
        logger.LogInformation("Alterando status do servico {ServicoId} para {Ativo}.", id, request.Ativo);
        var servico = await gateway.AlterarStatusAsync(id, request.Ativo);

        if (servico is null)
        {
            logger.LogWarning("Servico {ServicoId} nao encontrado para alteracao de status.", id);
            return Results.NotFound(new { message = "Servico nao encontrado." });
        }

        return Results.Ok(servico);
    }
    catch (Exception ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogError(ex, "Erro ao alterar status do servico {ServicoId}. OperationID={operationId}", id, operationId);
        return Results.Problem("Ocorreu um erro ao alterar o status do servico.");
    }
})
.WithName("UpdateServicoStatusViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapGet("/gateway/orcamentos", async (IApiGatewayService gateway, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Listando orcamentos pelo gateway.");
        var orcamentos = await gateway.GetOrcamentosAsync();
        return Results.Ok(orcamentos);
    }
    catch (Exception ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogError(ex, "Erro ao listar orcamentos pelo gateway. OperationID={operationId}", operationId);
        return Results.Problem("Ocorreu um erro ao listar os orcamentos.");
    }
})
.WithName("GetOrcamentosViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/gateway/orcamentos", async (CreateOrcamentoRequest? request, IApiGatewayService gateway, ILogger<Program> logger) =>
{
    var validationMessage = ValidateCreateOrcamentoRequest(request);
    if (validationMessage is not null)
    {
        var operationId = Guid.NewGuid();
        logger.LogWarning("Requisicao invalida para criar orcamento: {ValidationMessage}. OperationID={operationId}", validationMessage, operationId);
        return Results.BadRequest(new { message = validationMessage });
    }

    try
    {
        logger.LogInformation("Criando orcamento para servico {ServicoId}.", request!.ServicoId);
        var orcamento = await gateway.CriarOrcamentoAsync(request.ServicoId, request.ValorTotal);
        logger.LogInformation("Orcamento {OrcamentoId} criado para servico {ServicoId}.", orcamento.Id, orcamento.ServicoId);
        return Results.Created($"/gateway/orcamentos/{orcamento.Id}", orcamento);
    }
    catch (InvalidOperationException ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogWarning(ex, "Regra de negocio impediu a criacao do orcamento. OperationID={operationId}", operationId);
        return Results.BadRequest(new { message = ex.Message });
    }
    catch (ArgumentException ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogWarning(ex, "Dados invalidos ao criar orcamento. OperationID={operationId}", operationId);
        return Results.BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogError(ex, "Erro ao criar orcamento pelo gateway. OperationID={operationId}", operationId);
        return Results.Problem("Ocorreu um erro ao criar o orcamento.");
    }
})
.WithName("CreateOrcamentoViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapGet("/gateway/agenda", async (IApiGatewayService gateway, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Listando agenda pelo gateway.");
        var agenda = await gateway.GetAgendaAsync();
        return Results.Ok(agenda);
    }
    catch (Exception ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogError(ex, "Erro ao listar agenda pelo gateway. OperationID={operationId}", operationId);
        return Results.Problem("Ocorreu um erro ao listar a agenda.");
    }
})
.WithName("GetAgendaViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapGet("/gateway/historico", async (IApiGatewayService gateway, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Listando historico pelo gateway.");
        var historico = await gateway.GetHistoricoAsync();
        return Results.Ok(historico);
    }
    catch (Exception ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogError(ex, "Erro ao listar historico pelo gateway. OperationID={operationId}", operationId);
        return Results.Problem("Ocorreu um erro ao listar o historico.");
    }
})
.WithName("GetHistoricoViaGateway")
.WithOpenApi()
.RequireAuthorization();

app.MapPost("/gateway/auth/login", async (LoginRequest? request, IApiGatewayService gateway, ILogger<Program> logger) =>
{
    var validationMessage = ValidateLoginRequest(request);
    if (validationMessage is not null)
    {
        var operationId = Guid.NewGuid();
        logger.LogWarning("Requisicao invalida para login: {ValidationMessage}. OperationID={operationId}", validationMessage, operationId);
        return Results.BadRequest(new { message = validationMessage });
    }

    try
    {
        logger.LogInformation("Tentando login via provider {Provider}.", request!.Provider);
        var result = await gateway.LoginAsync(
            new AuthenticationRequest(request.Provider, request.Token, request.Email, request.Name));

        if (!result.IsAuthenticated || string.IsNullOrWhiteSpace(result.JwtToken))
        {
            logger.LogWarning("Falha de login via provider {Provider}: {Message}", request.Provider, result.Message);
            return Results.BadRequest(new { result.Message });
        }

        logger.LogInformation("Login realizado com sucesso para {Email} via provider {Provider}.", result.Email, result.Provider);
        return Results.Ok(new
        {
            result.JwtToken,
            result.Provider,
            result.Email
        });
    }
    catch (ArgumentException ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogWarning(ex, "Dados invalidos ao realizar login. OperationID={operationId}", operationId);
        return Results.BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        var operationId = Guid.NewGuid();
        logger.LogError(ex, "Erro ao realizar login via gateway. OperationID={operationId}", operationId);
        return Results.Problem("Ocorreu um erro ao realizar login.");
    }
})
.WithName("LoginViaGateway")
.WithOpenApi();

app.Run();

static string? ValidateCreateServicoRequest(CreateServicoRequest? request)
{
    if (request is null)
    {
        return "Informe os dados do servico.";
    }

    if (string.IsNullOrWhiteSpace(request.Nome))
    {
        return "O nome do servico e obrigatorio.";
    }

    if (request.Valor < 0)
    {
        return "O valor nao pode ser negativo.";
    }

    return null;
}

static string? ValidateCreateOrcamentoRequest(CreateOrcamentoRequest? request)
{
    if (request is null)
    {
        return "Informe os dados do orcamento.";
    }

    if (request.ServicoId == Guid.Empty)
    {
        return "Informe um servico valido para o orcamento.";
    }

    if (request.ValorTotal < 0)
    {
        return "O valor total nao pode ser negativo.";
    }

    return null;
}

static string? ValidateLoginRequest(LoginRequest? request)
{
    if (request is null)
    {
        return "Informe os dados de login.";
    }

    if (string.IsNullOrWhiteSpace(request.Provider))
    {
        return "O provider e obrigatorio.";
    }

    return null;
}

public record CreateServicoRequest(string Nome, string Descricao, decimal Valor);
public record CreateOrcamentoRequest(Guid ServicoId, decimal ValorTotal);
public record UpdateStatusRequest(bool Ativo);
public record LoginRequest(string Provider, string? Token, string? Email, string? Name = null);

public partial class Program;
