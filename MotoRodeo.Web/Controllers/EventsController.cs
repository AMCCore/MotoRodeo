using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRodeo.BL;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Dtos;
using MotoRodeo.DAL.Enums;
using MotoRodeo.Web.Models;

namespace MotoRodeo.Web.Controllers;

/// <summary>
/// Список, карточка, создание и редактирование событий, заявки и подтверждение участия.
/// </summary>
[Authorize]
public class EventsController(
    IMediator mediator,
    IAdvancedSecurityService security) : Controller
{
    /// <summary>
    /// Список событий: текущее, планируемые, прошедшие.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken token)
    {
        var list = await mediator.Send(new GetEventsListQuery(), token);
        return View(list);
    }

    /// <summary>
    /// Карточка события.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken token)
    {
        try
        {
            var details = await mediator.Send(new GetEventDetailsQuery(id), token);
            ViewBag.CanManage = security.HasRight(AccountRightEnum.ManageEvents);
            return View(details);
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException)
        {
            return View("Forbidden");
        }
    }

    /// <summary>
    /// Форма создания события.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken token)
    {
        if (!security.HasRight(AccountRightEnum.ManageEvents))
        {
            return View("Forbidden");
        }

        var now = DateTimeOffset.Now;
        var eventDate = now.AddDays(7);
        var closes = eventDate.AddDays(-EventOptions.RegistrationClosesDaysBefore);
        var candidates = await mediator.Send(new GetJudgeCandidatesQuery(), token);

        return View("Edit", new EventEditForm
        {
            EventDateLocal = ToLocalInput(eventDate),
            RegistrationClosesAtLocal = ToLocalInput(closes),
            GroupCount = 1,
            JudgeCandidates = candidates
        });
    }

    /// <summary>
    /// Форма редактирования события.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken token)
    {
        if (!security.HasRight(AccountRightEnum.ManageEvents))
        {
            return View("Forbidden");
        }

        try
        {
            var details = await mediator.Send(new GetEventDetailsQuery(id), token);
            var exclude = details.Participants.Select(p => p.AccountId).ToList();
            var candidates = await mediator.Send(new GetJudgeCandidatesQuery(exclude), token);

            // Уже назначенные судьи должны остаться в списке выбора даже если они в exclude не попали.
            var selected = details.Judges.Select(j => j.Id).ToList();
            var merged = candidates
                .Concat(details.Judges)
                .GroupBy(x => x.Id)
                .Select(g => g.First())
                .OrderBy(x => x.Name)
                .ToList();

            return View(new EventEditForm
            {
                Id = details.Id,
                Title = details.Title,
                Place = details.Place,
                EventDateLocal = ToLocalInput(details.EventDate),
                RegistrationClosesAtLocal = ToLocalInput(details.RegistrationClosesAt),
                GroupCount = details.GroupCount,
                JudgeAccountIds = selected,
                JudgeCandidates = merged
            });
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Сохранение создания или редактирования события.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EventEditForm form, CancellationToken token)
    {
        if (!security.HasRight(AccountRightEnum.ManageEvents))
        {
            return View("Forbidden");
        }

        form.JudgeAccountIds ??= [];
        form.JudgeCandidates = await LoadJudgeCandidatesForFormAsync(form, token);

        if (!ModelState.IsValid)
        {
            return View(form);
        }

        try
        {
            var eventDate = ToUtcOffset(form.EventDateLocal);
            var closesAt = ToUtcOffset(form.RegistrationClosesAtLocal);

            if (form.Id is null)
            {
                var id = await mediator.Send(new CreateEventCommand(
                    form.Title,
                    form.Place,
                    eventDate,
                    closesAt,
                    form.GroupCount,
                    form.JudgeAccountIds), token);
                TempData["Info"] = "Событие создано.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await mediator.Send(new UpdateEventCommand(
                form.Id.Value,
                form.Title,
                form.Place,
                eventDate,
                closesAt,
                form.GroupCount,
                form.JudgeAccountIds), token);
            TempData["Info"] = "Событие сохранено.";
            return RedirectToAction(nameof(Details), new { id = form.Id });
        }
        catch (UnauthorizedAccessException)
        {
            return View("Forbidden");
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        {
            form.Error = ex.Message;
            return View(form);
        }
    }

    /// <summary>
    /// Подача заявки на участие.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(ApplyToEventForm form, CancellationToken token)
    {
        try
        {
            await mediator.Send(new ApplyToEventCommand(form.EventId, form.UsesOwnEquipment), token);
            TempData["Info"] = "Заявка подана. Ожидайте подтверждения.";
        }
        catch (UnauthorizedAccessException)
        {
            return View("Forbidden");
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = form.EventId });
    }

    /// <summary>
    /// Подтверждение участия кандидата.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(Guid eventId, Guid accountId, CancellationToken token)
    {
        try
        {
            await mediator.Send(new ConfirmParticipantCommand(eventId, accountId), token);
            TempData["Info"] = "Участие подтверждено.";
        }
        catch (UnauthorizedAccessException)
        {
            return View("Forbidden");
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = eventId });
    }

    /// <summary>
    /// Отклонение заявки кандидата.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid eventId, Guid accountId, CancellationToken token)
    {
        try
        {
            await mediator.Send(new RejectParticipantCommand(eventId, accountId), token);
            TempData["Info"] = "Заявка отклонена.";
        }
        catch (UnauthorizedAccessException)
        {
            return View("Forbidden");
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = eventId });
    }

    private async Task<IReadOnlyList<NamedAccountDto>> LoadJudgeCandidatesForFormAsync(
        EventEditForm form,
        CancellationToken token)
    {
        IReadOnlyList<Guid>? exclude = null;
        if (form.Id is Guid eventId)
        {
            try
            {
                var details = await mediator.Send(new GetEventDetailsQuery(eventId), token);
                exclude = details.Participants.Select(p => p.AccountId).ToList();
                var candidates = await mediator.Send(new GetJudgeCandidatesQuery(exclude), token);
                return candidates
                    .Concat(details.Judges)
                    .GroupBy(x => x.Id)
                    .Select(g => g.First())
                    .OrderBy(x => x.Name)
                    .ToList();
            }
            catch (KeyNotFoundException)
            {
                // fall through
            }
        }

        return await mediator.Send(new GetJudgeCandidatesQuery(exclude), token);
    }

    private static DateTime ToLocalInput(DateTimeOffset value) =>
        DateTime.SpecifyKind(value.ToLocalTime().DateTime, DateTimeKind.Unspecified);

    private static DateTimeOffset ToUtcOffset(DateTime localUnspecified) =>
        new DateTimeOffset(DateTime.SpecifyKind(localUnspecified, DateTimeKind.Local)).ToUniversalTime();
}
