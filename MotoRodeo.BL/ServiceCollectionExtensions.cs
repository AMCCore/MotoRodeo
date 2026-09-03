using Microsoft.Extensions.DependencyInjection;
using MotoRodeo.BL.Services;
using System.Reflection;

namespace MotoRodeo.BL;

/// <summary>
/// Регистрация BL в DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Ключ лицензии для MediatR (берется из переменных окружения).
    /// </summary>
    private static string LuckypennyLicenseKey => Environment.GetEnvironmentVariable(nameof(LuckypennyLicenseKey)) ?? throw new ArgumentNullException(nameof(LuckypennyLicenseKey));

    /// <summary>
    /// Подключает сервисы бизнес-логики и MediatR.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Та же коллекция для цепочки вызовов.</returns>
    public static IServiceCollection AddMotoRodeoBl(this IServiceCollection services)
    {
        Assembly currentAssem = Assembly.GetExecutingAssembly();

        services.AddSingleton<IEmailSender, MailKitEmailSender>();
        services.AddMediatR(cfg =>
        {
            cfg.LicenseKey = LuckypennyLicenseKey;
            cfg.RegisterServicesFromAssembly(currentAssem);
        });
        return services;
    }
}