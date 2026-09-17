using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MotoRodeo.Web.Health;

/// <summary>
/// Минимальная проверка «приложение живо» без внешних зависимостей (Liveness).
/// </summary>
public class SimpleHealthCheck : IHealthCheck
{
    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(HealthCheckResult.Healthy("Application OK"));
    }
}