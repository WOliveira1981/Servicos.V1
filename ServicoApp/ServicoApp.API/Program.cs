using Microsoft.Extensions.DependencyInjection;
using ServicoApp.Application;
using ServicoApp.Application.Services;
using ServicoApp.Infrastructure;
using ServicoApp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Padrão Dependency Injection (Inversão de Controle): registra as dependências da aplicação.
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
.WithOpenApi();

app.MapPost("/servicos", async (CreateServicoRequest request, IServicoAppService service) =>
{
    var servico = await service.CreateAsync(request.Nome, request.Descricao, request.Valor);
    return Results.Created($"/servicos/{servico.Id}", servico);
})
.WithName("CreateServico")
.WithOpenApi();

app.Run();

public record CreateServicoRequest(string Nome, string Descricao, decimal Valor);
