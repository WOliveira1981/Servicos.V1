using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServicoApp.Domain.Factories;
using ServicoApp.Domain.Repositories;
using ServicoApp.Infrastructure.Events;
using ServicoApp.Infrastructure.Persistence;
using ServicoApp.Infrastructure.Repositories;

namespace ServicoApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=servicoapp.db";

        services.AddSingleton<ServicoFactory>();
        services.AddScoped(_ => new SqliteConnection(connectionString));
        services.AddScoped<SqliteUnitOfWork>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SqliteUnitOfWork>());
        services.AddScoped<IServicoRepository, SqliteServicoRepository>();
        services.AddScoped<IOrcamentoRepository, SqliteOrcamentoRepository>();
        services.AddScoped<SqliteDatabaseInitializer>();
        services.AddEventDriven();

        return services;
    }
}