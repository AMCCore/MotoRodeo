using DMCorp.Framework.Basics.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.Web.Models;
using System.Security.Claims;
using System.Text.Json;

namespace MotoRodeo.Web.Controllers;

/// <summary>
/// Регистрация, вход, выход и запрос восстановления пароля.
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
    /// Создаёт учётную запись и отправляет письмо для подтверждения регистрации.
    /// </summary>
    /// <param name="form">Данные формы регистрации.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Форма с ошибкой или сообщение о необходимости подтвердить почту.</returns>
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
            await mediator.Send(new RegisterUserCommand(
                form.FirstName,
                form.LastName,
                form.Nickname,
                form.Email,
                form.Password,
                id => Url.Action(
                    "ConfirmRegistration",
                    "Accounts",
                    new { id },
                    Request.Scheme)!), token);

            return View(new RegisterForm
            {
                Info = "Учётная запись создана. Проверьте электронную почту для активации."
            });
        }
        catch (Exception ex)
        {
            form.Error = ex.Message;
            form.Password = string.Empty;
            return View(form);
        }
    }

    /// <summary>
    /// Отображает форму запроса восстановления пароля.
    /// </summary>
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Events");
        }

        return View(new ForgotPasswordForm());
    }

    /// <summary>
    /// Создаёт запрос восстановления пароля и отправляет ссылку на почту, если учётная запись найдена.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordForm form, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        await mediator.Send(new RequestPasswordResetCommand(form.Email, id => Url.Action("ConfirmPasswordReset", "Accounts", new { id }, Request.Scheme)!), token);
        form.Info = "Cсылка для восстановления пароля отправлена на указанный email.";
        return View(form);
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
            new(ClaimTypes.Name, account.Login)
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
