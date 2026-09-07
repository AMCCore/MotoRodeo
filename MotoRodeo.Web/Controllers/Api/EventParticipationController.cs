using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.Web.Filters;

namespace MotoRodeo.Web.Controllers.Api;

/// <summary>
/// Внешнее API подтверждения и отклонения участия в событии.
/// </summary>
[ApiController]
[AllowAnonymous]
[EventParticipationApiKey]
[Route("api/events/{eventId:guid}/participants/{accountId:guid}")]
public sealed class EventParticipationController(IMediator mediator, ILogger<EventParticipationController> logger) : ControllerBase
{
    /// <summary>
    /// Подтверждает участие кандидата.
    /// </summary>
    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm(Guid eventId, Guid accountId, CancellationToken token)
    {
        try
        {
            await mediator.Send(new ConfirmParticipantCommand(eventId, accountId, IsExternalApi: true), token);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "API confirm: заявка не найдена. EventId={EventId}, AccountId={AccountId}", eventId, accountId);
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "API confirm: недопустимый переход. EventId={EventId}, AccountId={AccountId}", eventId, accountId);
            return Conflict(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Отклоняет заявку кандидата.
    /// </summary>
    [HttpPost("reject")]
    public async Task<IActionResult> Reject(Guid eventId, Guid accountId, CancellationToken token)
    {
        try
        {
            await mediator.Send(new RejectParticipantCommand(eventId, accountId, IsExternalApi: true), token);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning(ex, "API reject: заявка не найдена. EventId={EventId}, AccountId={AccountId}", eventId, accountId);
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "API reject: недопустимый переход. EventId={EventId}, AccountId={AccountId}", eventId, accountId);
            return Conflict(new { error = ex.Message });
        }
    }
}
