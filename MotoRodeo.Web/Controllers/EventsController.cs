using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MotoRodeo.BL;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Enums;
using MotoRodeo.Web.Models;

namespace MotoRodeo.Web.Controllers;

/// <summary>
/// Просмотр и управление соревновательными событиями.
/// </summary>
[Authorize]
public class EventsController(IMediator mediator, IAdvancedSecurityService security, IOptions<EventOptions> eventOptions) : Controller
{
    /// <summary>
    /// Отображает список событий с учётом прав текущего пользователя.
    /// </summary>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Представление со списком событий.</returns>
    public async Task<IActionResult> Index(CancellationToken token)
    {
        var items = await mediator.Send(new GetEventsQuery(), token);
        ViewBag.CanManage = security.HasRight(AccountRightEnum.ManageEvents);
        ViewBag.CanParticipate = security.HasRight(AccountRightEnum.CanParticipate);
        return View(items);
    }

    /// <summary>
    /// Отображает подробности события: участники, судьи, расписание и сетку.
    /// </summary>
    /// <param name="id">Идентификатор события.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Представление с деталями события.</returns>
    public async Task<IActionResult> Details(Guid id, CancellationToken token)
    {
        var details = await mediator.Send(new GetEventDetailsQuery(id), token);
        ViewBag.CanManage = security.HasRight(AccountRightEnum.ManageEvents);
        ViewBag.CanParticipate = security.HasRight(AccountRightEnum.CanParticipate);
        ViewBag.IsAdmin = security.IsAdmin;
        return View(details);
    }

    /// <summary>
    /// Открывает пустую форму создания нового события.
    /// </summary>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Представление редактирования с пустой формой.</returns>
    public async Task<IActionResult> Create(CancellationToken token)
    {
        EnsureManage();
        ViewBag.DefaultRegistrationClosesDaysBefore = eventOptions.Value.DefaultRegistrationClosesDaysBefore;
        return View("Edit", await EmptyForm(token));
    }

    /// <summary>
    /// Создаёт новое событие по данным формы.
    /// </summary>
    /// <param name="form">Параметры создаваемого события.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Форма с ошибкой или перенаправление к деталям созданного события.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventEditForm form, CancellationToken token)
    {
        EnsureManage();
        form.JudgeCandidates = await mediator.Send(new GetJudgeCandidatesQuery(), token);
        ViewBag.DefaultRegistrationClosesDaysBefore = eventOptions.Value.DefaultRegistrationClosesDaysBefore;
        if (!ModelState.IsValid)
        {
            return View("Edit", form);
        }

        try
        {
            var id = await mediator.Send(ToCreate(form), token);
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (DomainException ex)
        {
            form.Error = ex.Message;
            return View("Edit", form);
        }
    }

    /// <summary>
    /// Открывает форму редактирования существующего события.
    /// </summary>
    /// <param name="id">Идентификатор события.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Представление редактирования с заполненной формой.</returns>
    public async Task<IActionResult> Edit(Guid id, CancellationToken token)
    {
        EnsureManage();
        var details = await mediator.Send(new GetEventDetailsQuery(id), token);
        var form = await EmptyForm(token);
        form.Id = details.Id;
        form.Title = details.Title;
        form.Place = details.Place;
        form.EventDateLocal = details.EventDate.DateTime;
        form.RegistrationClosesAtLocal = details.RegistrationDeadline.DateTime;
        form.GroupCount = details.GroupCount;
        form.JudgeIds = details.Judges.Select(x => x.Id).ToList();
        return View(form);
    }

    /// <summary>
    /// Сохраняет изменения существующего события.
    /// </summary>
    /// <param name="form">Обновлённые параметры события.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Форма с ошибкой или перенаправление к деталям события.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EventEditForm form, CancellationToken token)
    {
        EnsureManage();
        form.JudgeCandidates = await mediator.Send(new GetJudgeCandidatesQuery(), token);
        if (form.Id is null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(form);
        }

        try
        {
            await mediator.Send(new UpdateEventCommand(
                form.Id.Value,
                form.Title,
                form.Place,
                new DateTimeOffset(DateTime.SpecifyKind(form.EventDateLocal, DateTimeKind.Local)),
                ToOffset(form.RegistrationClosesAtLocal) ?? throw new DomainException("Укажите дату окончания регистрации."),
                form.GroupCount,
                form.JudgeIds), token);
            return RedirectToAction(nameof(Details), new { id = form.Id });
        }
        catch (DomainException ex)
        {
            form.Error = ex.Message;
            return View(form);
        }
    }

    /// <summary>
    /// Подтверждает участие текущего пользователя в событии.
    /// </summary>
    /// <param name="id">Идентификатор события.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Перенаправление к деталям события.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken token)
    {
        await mediator.Send(new ConfirmEventParticipationCommand(id), token);
        return RedirectToAction(nameof(Details), new { id });
    }

    /// <summary>
    /// Отменяет участие текущего пользователя в событии.
    /// </summary>
    /// <param name="id">Идентификатор события.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Перенаправление к деталям события.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken token)
    {
        await mediator.Send(new CancelEventParticipationCommand(id), token);
        return RedirectToAction(nameof(Details), new { id });
    }

    /// <summary>
    /// Закрывает регистрацию и формирует сетку соревнования.
    /// </summary>
    /// <param name="id">Идентификатор события.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Перенаправление к деталям события.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BuildGrid(Guid id, CancellationToken token)
    {
        EnsureManage();
        await mediator.Send(new CloseRegistrationAndBuildGridCommand(id), token);
        return RedirectToAction(nameof(Details), new { id });
    }

    /// <summary>
    /// Экспортирует список участников события в CSV-файл.
    /// </summary>
    /// <param name="id">Идентификатор события.</param>
    /// <param name="grouped">Признак группировки участников по группам в выгрузке.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Файл CSV для скачивания.</returns>
    public async Task<IActionResult> Export(Guid id, bool grouped, CancellationToken token)
    {
        var file = await mediator.Send(new ExportParticipantsCsvQuery(id, grouped), token);
        return File(file.Content, "text/csv", file.FileName);
    }

    private void EnsureManage()
    {
        if (!security.HasRight(AccountRightEnum.ManageEvents))
        {
            throw new UnauthorizedAccessException("Недостаточно прав.");
        }
    }

    private EventEditForm EmptyForm(IReadOnlyList<MotoRodeo.BL.Dtos.NamedAccountDto> judgeCandidates)
    {
        var eventDate = DateTime.Now.AddDays(14);
        var daysBefore = eventOptions.Value.DefaultRegistrationClosesDaysBefore;
        return new EventEditForm
        {
            JudgeCandidates = judgeCandidates,
            EventDateLocal = eventDate,
            RegistrationClosesAtLocal = eventDate.AddDays(-daysBefore)
        };
    }

    private async Task<EventEditForm> EmptyForm(CancellationToken token)
    {
        var judgeCandidates = await mediator.Send(new GetJudgeCandidatesQuery(), token);
        return EmptyForm(judgeCandidates);
    }

    private CreateEventCommand ToCreate(EventEditForm form)
    {
        var eventDate = new DateTimeOffset(DateTime.SpecifyKind(form.EventDateLocal, DateTimeKind.Local));
        return new(
            form.Title,
            form.Place,
            eventDate,
            ToOffset(form.RegistrationClosesAtLocal),
            form.GroupCount,
            form.JudgeIds);
    }

    private static DateTimeOffset? ToOffset(DateTime? local) =>
        local is { } value && value != default
            ? new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Local))
            : null;
}