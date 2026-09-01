using System.Security.Claims;
using System.Text.Json;
using DMCorp.Framework.Basics.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.Web.Models;

namespace MotoRodeo.Web.Controllers;

/// <summary>
/// Регистрация, вход и выход пользователей.
/// </summary>
public class AccountController(IMediator mediator) : Controller
{
    /// <summary>
    /// Отображает форму входа или перенаправляет уже аутентифицированного пользователя.
    /// </summary>
    /// <param name="returnUrl">Локальный URL для возврата после успешного входа.</param>
    /// <returns>Форма входа или перенаправление к списку событий.</returns>
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Events");
        }

        return View(new LoginForm { ReturnUrl = returnUrl });
    }

    /// <summary>
    /// Проверяет учётные данные и выполняет вход по cookie-аутентификации.
    /// </summary>
    /// <param name="form">Данные формы входа.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Форма с ошибкой, перенаправление на returnUrl или к списку событий.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginForm form, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var login = await mediator.Send(new GetMainAccountLoginQuery(form.Login.Trim()), token);
        if (login == null || !BCrypt.Net.BCrypt.Verify(form.Password, login.Password))
        {
            form.Password = string.Empty;
            form.Error = "Неверный логин или пароль.";
            return View(form);
        }

        await SignInAsync(login.AccountId, form.RememberMe, token);
        if (!string.IsNullOrWhiteSpace(form.ReturnUrl) && Url.IsLocalUrl(form.ReturnUrl))
        {
            return Redirect(form.ReturnUrl);
        }

        return RedirectToAction("Index", "Events");
    }

    /// <summary>
    /// Отображает форму регистрации нового пользователя.
    /// </summary>
    /// <returns>Форма регистрации или перенаправление к списку событий.</returns>
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Events");
        }

        return View(new RegisterForm());
    }

    /// <summary>
    /// Создаёт учётную запись и выполняет автоматический вход.
    /// </summary>
    /// <param name="form">Данные формы регистрации.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Форма с ошибкой или перенаправление к списку событий.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterForm form, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        try
        {
            var id = await mediator.Send(new RegisterUserCommand(form.Name, form.Login, form.Password), token);
            await SignInAsync(id, false, token);
            return RedirectToAction("Index", "Events");
        }
        catch (DomainException ex)
        {
            form.Error = ex.Message;
            form.Password = string.Empty;
            return View(form);
        }
    }

    /// <summary>
    /// Завершает сеанс пользователя и перенаправляет на страницу входа.
    /// </summary>
    /// <returns>Перенаправление на страницу входа.</returns>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    private async Task SignInAsync(Guid accountId, bool persistent, CancellationToken token)
    {
        var account = await mediator.Send(new GetAccountWithRightsCommand(accountId), token);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, JsonSerializer.Serialize(account.Rights.Select(a => a.GetEnumGuid()))),
            new(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
            new(ClaimTypes.Name, account.Name)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, null);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = persistent,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
            });
    }
}