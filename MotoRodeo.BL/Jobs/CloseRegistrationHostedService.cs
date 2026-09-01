using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Events;
using MediatR;

namespace MotoRodeo.BL.Jobs;

/// <summary>
/// Периодически закрывает регистрацию по дедлайну. Без Hangfire.
/// </summary>
public sealed class CloseRegistrationHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<CloseRegistrationHostedService> logger) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new CloseDueEventsCommand(), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при закрытии регистрации по расписанию.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}