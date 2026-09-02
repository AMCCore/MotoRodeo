using Microsoft.Extensions.DependencyInjection;
using MotoRodeo.BL.Services;

namespace MotoRodeo.BL;

/// <summary>
/// Регистрация BL в DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Подключает сервисы бизнес-логики и MediatR.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Та же коллекция для цепочки вызовов.</returns>
    public static IServiceCollection AddMotoRodeoBl(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IParticipantShuffler, CryptographicParticipantShuffler>();
        services.AddScoped<EventGridBuilder>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
        return services;
    }
}