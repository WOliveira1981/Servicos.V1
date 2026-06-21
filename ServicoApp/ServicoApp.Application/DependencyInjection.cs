using Microsoft.Extensions.DependencyInjection;
using ServicoApp.Application.Gateway;
using ServicoApp.Application.Services;

namespace ServicoApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IServicoAppService, ServicoAppService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IApiGatewayService, ApiGatewayService>();
        return services;
    }
}
