using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MotoRodeo.Web.Health;

/// <summary>
/// Проверка работоспособности соединения с базой данных.
/// Используется для Readiness probe в Kubernetes/Docker.
/// </summary>
public class SimpleDbCheck(IUnitOfWork unitOfWork, ILogger<SimpleDbCheck>? logger = default) : IHealthCheck
{
    /// <summary>
    /// Выполняет проверку доступности базы данных через простой SQL-запрос.
    /// </summary>
    /// <param name="context">Контекст проверки здоровья.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат проверки: Healthy при успешном подключении, Unhealthy при ошибке.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            logger?.LogDebug("Checking DB connection...");
            await unitOfWork.Context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);

            logger?.LogDebug("DB connection check passed");
            return HealthCheckResult.Healthy("Database reachable");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "DB connection check failed");
            return HealthCheckResult.Unhealthy("Database failure", ex);
        }
    }
}