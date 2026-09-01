using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MotoRodeo.Web.Controllers;

/// <summary>
/// Точка входа и страница ошибок приложения.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Перенаправляет аутентифицированного пользователя к списку событий, иначе — на страницу входа.
    /// </summary>
    /// <returns>Перенаправление на список событий или страницу входа.</returns>
    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Events");
        }

        return RedirectToAction("Login", "Account");
    }

    /// <summary>
    /// Отображает страницу ошибки с сообщением из параметра, TempData или значением по умолчанию.
    /// </summary>
    /// <param name="message">Текст ошибки для отображения пользователю.</param>
    /// <returns>Представление страницы ошибки.</returns>
    [AllowAnonymous]
    public IActionResult Error(string? message)
    {
        ViewBag.Message = message ?? TempData["Error"] as string ?? "Произошла ошибка.";
        return View();
    }
}