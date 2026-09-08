using DMCorp.Framework.Basics.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Dtos;
using MotoRodeo.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;

namespace MotoRodeo.Web.Controllers;

/// <summary>
/// Регистрация, вход, выход и запрос восстановления пароля.
/// </summary>
public class AccountController(IMediator mediator, ILogger<AccountController> logger) : Controller
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

        var normalizedLogin = form.Login.Trim();
        logger.LogInformation("Начало входа. Login={Login}", normalizedLogin);

        var login = await mediator.Send(new GetMainAccountLoginQuery(normalizedLogin), token);
        if (login == null || !BCrypt.Net.BCrypt.Verify(form.Password, login.Password))
        {
            logger.LogWarning("Неудачная попытка входа. Login={Login}", normalizedLogin);
            form.Password = string.Empty;
            form.Error = "Неверный логин или пароль.";
            return View(form);
        }

        await SignInAsync(login.AccountId, form.RememberMe, token);
        logger.LogInformation("Пользователь вошёл в систему. AccountId={AccountId}", login.AccountId);

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

        logger.LogInformation("Начало регистрации. Email={Email}", form.Email);

        try
        {
            await mediator.Send(new RegisterUserCommand(
                form.FirstName,
                form.LastName,
                form.Nickname,
                form.Email,
                form.Password,
                form.Vehicle,
                id => Url.Action(
                    nameof(ConfirmRegistration),
                    "Account",
                    new { AccountId = id },
                    Request.Scheme)!), token);

            return View(new RegisterForm
            {
                Info = "Учётная запись создана. Проверьте электронную почту для активации."
            });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Ошибка регистрации. Email={Email}", form.Email);
            form.Error = ex.Message;
            form.Password = string.Empty;
            return View(form);
        }
    }

    /// <summary>
    /// Подтверждает регистрацию по ссылке из письма.
    /// </summary>
    /// <param name="AccountId">Идентификатор учётной записи.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Страница успеха или общей ошибки подтверждения.</returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("Confirm/{AccountId}")]
    public async Task<IActionResult> ConfirmRegistration([Required] Guid AccountId, CancellationToken token = default)
    {
        logger.LogInformation("Начало подтверждения регистрации. AccountId={AccountId}", AccountId);

        try
        {
            await mediator.Send(new ConfirmRegistrationCommand(AccountId), token);
            return View(new ConfirmRegistrationModel
            {
                Success = true,
                Message = "Учётная запись подтверждена."
            });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Ошибка подтверждения регистрации. AccountId={AccountId}", AccountId);
            return View(new ConfirmRegistrationModel
            {
                Success = false,
                Message = "Не удалось подтвердить регистрацию."
            });
        }
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("ConfirmPasswordReset/{ResetRequestId}")]
    public async Task<IActionResult> ConfirmPasswordReset(Guid ResetRequestId, CancellationToken token = default)
    {
        logger.LogInformation("Начало процедуры сброса пароля. ResetRequestId={ResetRequestId}", ResetRequestId);
        try
        {
            await mediator.Send(new ConfirmPasswordResetCommand(ResetRequestId), token);
            return View(new ConfirmRegistrationModel
            {
                Success = true,
                Message = "Пароль для данного пользователя сброшен и отправлен ему на email указанный при регистрации."
            });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Ошибка процедуры сброса пароля. ResetRequestId={ResetRequestId}", ResetRequestId);
            return View(new ConfirmRegistrationModel
            {
                Success = false,
                Message = "Не удалось сбросить пароль. Попробуйте позже."
            });
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

        await mediator.Send(new RequestPasswordResetCommand(form.Email, id => Url.Action("ConfirmPasswordReset", "Account", new { id }, Request.Scheme)!), token);
        form.Info = "Cсылка для восстановления пароля отправлена на указанный email.";
        return View(form);
    }

    /// <summary>
    /// Завершает сеанс пользователя и перенаправляет на страницу входа.
    /// </summary>
    /// <returns>Перенаправление на страницу входа.</returns>
    [Authorize]
    [HttpGet]
    [Route("/Logout")]
    public async Task<IActionResult> Logout()
    {
        var accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        logger.LogInformation("Пользователь вышел из системы. AccountId={AccountId}", accountId);
        return RedirectToAction(nameof(Login));
    }

    /// <summary>
    /// Отображает профиль текущего пользователя.
    /// </summary>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile(CancellationToken token)
    {
        var profile = await mediator.Send(new GetMyProfileQuery(), token);
        return View(ToProfileForm(profile));
    }

    /// <summary>
    /// Сохраняет изменения профиля текущего пользователя.
    /// </summary>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileForm form, CancellationToken token)
    {
        // Email не редактируется — подставляем актуальное значение из БД.
        var current = await mediator.Send(new GetMyProfileQuery(), token);
        form.Email = current.Email;

        if (!ModelState.IsValid)
        {
            return View(form);
        }

        try
        {
            await mediator.Send(new UpdateMyProfileCommand(
                form.FirstName,
                form.LastName,
                form.Nickname,
                form.Vehicle), token);

            form.Info = "Профиль сохранён.";
            return View(form);
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        {
            logger.LogWarning(ex, "Ошибка сохранения профиля.");
            form.Error = ex.Message;
            return View(form);
        }
    }

    /// <summary>
    /// Форма смены пароля.
    /// </summary>
    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordForm());
    }

    /// <summary>
    /// Меняет пароль текущего пользователя.
    /// </summary>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordForm form, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        try
        {
            await mediator.Send(new ChangePasswordCommand(form.CurrentPassword, form.NewPassword), token);
            form = new ChangePasswordForm { Info = "Пароль изменён." };
            return View(form);
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        {
            logger.LogWarning(ex, "Ошибка смены пароля.");
            form.CurrentPassword = string.Empty;
            form.NewPassword = string.Empty;
            form.ConfirmPassword = string.Empty;
            form.Error = ex.Message;
            return View(form);
        }
    }

    private static ProfileForm ToProfileForm(MyProfileDto profile) => new()
    {
        FirstName = profile.FirstName,
        LastName = profile.LastName,
        Nickname = profile.Nickname,
        Email = profile.Email,
        Vehicle = profile.Vehicle
    };

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