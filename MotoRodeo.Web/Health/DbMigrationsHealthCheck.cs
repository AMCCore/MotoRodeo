using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MotoRodeo.Web.Health;

/// <summary>
/// Сверяет применённые миграции EF с ожидаемыми: при наличии неприменённых возвращает <see cref="HealthStatus.Degraded"/> (деплой/схема не синхронизированы).
/// </summary>
public class DbMigrationsHealthCheck(IUnitOfWork unitOfWork, ILogger<DbMigrationsHealthCheck>? logger = default) : IHealthCheck
{
    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var applied = await unitOfWork.Context.Database.GetAppliedMigrationsAsync(cancellationToken: cancellationToken);

            var pending = await unitOfWork.Context.Database.GetPendingMigrationsAsync(cancellationToken);

            if (pending.Any())
            {
                return HealthCheckResult.Degraded("Есть неприменённые миграции.",
                    data: new Dictionary<string, object>
                    {
                        { "PendingMigrations", pending },
                        { "LatestAppliedMigration", applied.LastOrDefault() ?? "" }
                    });
            }

            return HealthCheckResult.Healthy(
                data: new Dictionary<string, object>
                {
                    { "LatestAppliedMigration", applied.LastOrDefault() ?? "" }
                });
        }
        catch (Exception ex)
        {
            logger?.LogDebug(ex, "DbMigrationsHealthChecks failure");
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}