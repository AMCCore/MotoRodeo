using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MotoRodeo.DAL.Context;

namespace MotoRodeo.DAL;

/// <summary>
/// Регистрация DAL в DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует контекст базы данных и единицу работы в контейнере DI.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурация приложения (строка подключения ConnectionStrings:MotoRodeo или переменная DbConnection).</param>
    /// <returns>Коллекция сервисов для цепочки вызовов.</returns>
    public static IServiceCollection AddMotoRodeoDal(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MotoRodeo")
            ?? Environment.GetEnvironmentVariable("DbConnection")
            ?? throw new InvalidOperationException("Не задана строка подключения ConnectionStrings:MotoRodeo или DbConnection.");

        services.AddDbContext<MotoRodeoContext>(options => options
            .UseLazyLoadingProxies()
            .UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}