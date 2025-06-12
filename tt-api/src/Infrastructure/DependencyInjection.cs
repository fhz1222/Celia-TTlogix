using Application.Interfaces.Gateways;
using Infrastructure.Email;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IILogConnectGateway, ILogConnectGateway>();
        services.AddScoped<IExcelWriter, ExcelWriter>();
        services.AddScoped<INotificationGateway, NotificationGateway>();
        return services;
    }
}