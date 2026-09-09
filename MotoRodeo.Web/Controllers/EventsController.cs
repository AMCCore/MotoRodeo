using System.Text;
using DMCorp.Framework.Basics.Extensions;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRodeo.BL;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Dtos;
using MotoRodeo.DAL.Enums;
using MotoRodeo.Web.Infrastructure;
using MotoRodeo.Web.Models;

namespace MotoRodeo.Web.Controllers;

/// <summary>
/// Список, карточка, создание и редактирование событий, заявки и подтверждение участия.
/// </summary>
[Authorize]
[Route("[controller]")]
public class EventsController(
    IMediator mediator,
    IAdvancedSecurityService security) : Controller
{
    /// <summary>
    /// Список событий: проводится, планируемые, прошедшие.
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index(CancellationToken token)
    {
        var list = await mediator.Send(new GetEventsListQuery(), token);
        return View(list);
    }

    /// <summary>
    /// Карточка события.
    /// </summary>
    [HttpGet]
    [Route("/Event/{id}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken token)
    {
        try
        {
            var details = await mediator.Send(new GetEventDetailsQuery(id), token);
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
    [Route("/Create")]
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
            Title = "МотоРодео",
            Place = "Мотошкола Дзен",
            EventDateLocal = ToLocalInput(eventDate),
            RegistrationClosesAtLocal = ToLocalInput(closes),
            JudgeCandidates = candidates
        });
    }

    /// <summary>
    /// Форма редактирования события.
    /// </summary>
    [HttpGet]
    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken token)
    {
        if (!security.HasRight(AccountRightEnum.ManageEvents))
        {
            return View("Forbidden");
        }

        try
        {
            var details = await mediator.Send(new GetEventDetailsQuery(id), token);
            if (!details.CanEditEvent)
            {
                TempData["Error"] = "Завершённое мероприятие нельзя изменять.";
                return RedirectToAction(nameof(Details), new { id });
            }

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
    [Route("Edit")]
    [Route("Edit/{id:guid}")]
    public async Task<IActionResult> Save(EventEditForm form, CancellationToken token)
    {
        if (!security.HasRight(AccountRightEnum.ManageEvents))
        {
            return View("Forbidden");
        }

        form.JudgeAccountIds ??= [];
        form.JudgeCandidates = await LoadJudgeCandidatesForFormAsync(form, token);

        if (!ModelState.IsValid)
        {
            return View("Edit", form);
        }

        try
        {
            var eventDate = AppTimeZone.ToUtc(form.EventDateLocal);
            var closesAt = AppTimeZone.ToUtcEndOfDay(form.RegistrationClosesAtLocal);

            if (form.Id.IsNullOrEmpty())
            {
                var id = await mediator.Send(new CreateEventCommand(
                    form.Title,
                    form.Place,
                    eventDate,
                    closesAt,
                    form.JudgeAccountIds), token);
                TempData["Info"] = "Событие создано.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await mediator.Send(new UpdateEventCommand(
                form.Id!.Value,
                form.Title,
                form.Place,
                eventDate,
                closesAt,
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
            return View("Edit", form);
        }
    }

    /// <summary>
    /// Подача заявки на участие.
    /// </summary>
    [HttpPost]
    [Route("Apply")]
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
    [Route("Confirm")]
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
    [Route("Reject")]
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

    /// <summary>
    /// Начать мероприятие (планируемое → проводится).
    /// </summary>
    [HttpPost]
    [Route("Start")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(Guid eventId, CancellationToken token)
    {
        try
        {
            await mediator.Send(new StartEventCommand(eventId), token);
            TempData["Info"] = "Мероприятие начато.";
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
    /// Завершить мероприятие.
    /// </summary>
    [HttpPost]
    [Route("Complete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(Guid eventId, CancellationToken token)
    {
        try
        {
            await mediator.Send(new CompleteEventCommand(eventId), token);
            TempData["Info"] = "Мероприятие завершено.";
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
    /// Отменить планируемое мероприятие.
    /// </summary>
    [HttpPost]
    [Route("Cancel")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid eventId, CancellationToken token)
    {
        try
        {
            await mediator.Send(new CancelEventCommand(eventId), token);
            TempData["Info"] = "Мероприятие отменено.";
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
    /// Выгрузка подтверждённых участников: печать (view) или CSV-файл (file).
    /// </summary>
    [HttpGet]
    [Route("/Event/{id}/participants/export")]
    public async Task<IActionResult> ExportParticipants(
        Guid id,
        string format = "view",
        CancellationToken token = default)
    {
        try
        {
            var export = await mediator.Send(new GetConfirmedParticipantsExportQuery(id), token);
            if (string.Equals(format, "file", StringComparison.OrdinalIgnoreCase))
            {
                var bytes = BuildParticipantsCsv(export);
                var fileName = BuildExportFileName(export);
                return File(bytes, "text/csv; charset=utf-8", fileName);
            }

            return View(export);
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (UnauthorizedAccessException)
        {
            return View("Forbidden");
        }
    }

    private static byte[] BuildParticipantsCsv(ConfirmedParticipantsExportDto export)
    {
        var sb = new StringBuilder();
        sb.AppendLine(CsvCell(export.Title));
        sb.AppendLine(CsvCell(AppTimeZone.ToLocal(export.EventDate).ToString("dd.MM.yyyy HH:mm")));
        sb.AppendLine();
        sb.AppendLine(string.Join(';', "№", "Фамилия Имя (Прозвище)", "Мотоцикл"));

        foreach (var row in export.Rows)
        {
            sb.AppendLine(string.Join(';',
                CsvCell(row.RowNumber.ToString()),
                CsvCell(row.DisplayName),
                CsvCell(row.Motorcycle)));
        }

        var preamble = Encoding.UTF8.GetPreamble();
        var content = Encoding.UTF8.GetBytes(sb.ToString());
        var result = new byte[preamble.Length + content.Length];
        Buffer.BlockCopy(preamble, 0, result, 0, preamble.Length);
        Buffer.BlockCopy(content, 0, result, preamble.Length, content.Length);
        return result;
    }

    private static string CsvCell(string value)
    {
        if (value.Contains('"') || value.Contains(';') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }

    private static string BuildExportFileName(ConfirmedParticipantsExportDto export)
    {
        var date = AppTimeZone.ToLocal(export.EventDate).ToString("yyyy-MM-dd");
        var title = string.Join("_", export.Title.Split(
            Path.GetInvalidFileNameChars(),
            StringSplitOptions.RemoveEmptyEntries)).Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            title = "event";
        }

        return $"uchastniki_{title}_{date}.csv";
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
        AppTimeZone.ToLocal(value);
}