using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.Web.Controllers;

/// <summary>
/// Администрирование пользователей системы.
/// </summary>
[Authorize]
[Route("[controller]")]
public class AccountsController(
    IMediator mediator,
    IAdvancedSecurityService security,
    ILogger<AccountsController> logger) : Controller
{
    /// <summary>
    /// Полный список пользователей с клиентским поиском.
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index(CancellationToken token)
    {
        if (!security.HasRight(AccountRightEnum.ManageEvents))
        {
            return View("Forbidden");
        }

        var list = await mediator.Send(new GetAccountsListQuery(), token);
        return View(list);
    }

    /// <summary>
    /// Полная карточка пользователя.
    /// </summary>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken token)
    {
        if (!security.HasRight(AccountRightEnum.ManageEvents))
        {
            return View("Forbidden");
        }

        try
        {
            var details = await mediator.Send(new GetAccountDetailsQuery(id), token);
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
    /// Блокирует учётную запись.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:guid}/Block")]
    public async Task<IActionResult> Block(Guid id, CancellationToken token)
    {
        if (!security.HasRight(AccountRightEnum.ManageEvents))
        {
            return View("Forbidden");
        }

        try
        {
            await mediator.Send(new BlockAccountCommand(id), token);
            TempData["Info"] = "Учётная запись заблокирована.";
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException or UnauthorizedAccessException)
        {
            logger.LogWarning(ex, "Ошибка блокировки. AccountId={AccountId}", id);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    /// <summary>
    /// Разблокирует учётную запись.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("{id:guid}/Unblock")]
    public async Task<IActionResult> Unblock(Guid id, CancellationToken token)
    {
        if (!security.HasRight(AccountRightEnum.ManageEvents))
        {
            return View("Forbidden");
        }

        try
        {
            await mediator.Send(new UnblockAccountCommand(id), token);
            TempData["Info"] = "Учётная запись разблокирована.";
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException or UnauthorizedAccessException)
        {
            logger.LogWarning(ex, "Ошибка разблокировки. AccountId={AccountId}", id);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}