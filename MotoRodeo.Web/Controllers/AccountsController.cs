using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.DAL.Enums;
using MotoRodeo.Web.Models;

namespace MotoRodeo.Web.Controllers;

/// <summary>
/// Управление учётными записями и правами доступа (только для авторизованных пользователей).
/// </summary>
[Authorize]
public class AccountsController(IMediator mediator) : Controller
{
    /// <summary>
    /// Отображает список учётных записей с текущими правами.
    /// </summary>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Представление со списком учётных записей.</returns>
    public async Task<IActionResult> Index(CancellationToken token)
    {
        var items = await mediator.Send(new GetAccountsQuery(), token);
        return View(items);
    }

    /// <summary>
    /// Сохраняет набор прав для указанной учётной записи.
    /// </summary>
    /// <param name="form">Идентификатор учётной записи и выбранные права.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Перенаправление к списку учётных записей.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(AccountRightsForm form, CancellationToken token)
    {
        try
        {
            await mediator.Send(new SetAccountRightsCommand(form.AccountId, form.Rights ?? []), token);
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Полный перечень доступных прав для отображения в форме редактирования.
    /// </summary>
    public IReadOnlyList<AccountRightEnum> AllRights => Enum.GetValues<AccountRightEnum>();
}