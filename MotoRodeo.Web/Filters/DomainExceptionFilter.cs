using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MotoRodeo.BL.Exceptions;

namespace MotoRodeo.Web.Filters;

/// <summary>
/// Преобразует доменные исключения и отказ в доступе в HTTP-ответы.
/// </summary>
public sealed class DomainExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// Обрабатывает <see cref="UnauthorizedAccessException"/> как 403 и <see cref="DomainException"/> как страницу ошибки.
    /// </summary>
    /// <param name="context">Контекст необработанного исключения MVC.</param>
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case UnauthorizedAccessException ex:
                var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
                {
                    ["Message"] = string.IsNullOrWhiteSpace(ex.Message) ? "Недостаточно прав." : ex.Message
                };
                context.Result = new ViewResult
                {
                    ViewName = "Forbidden",
                    ViewData = viewData,
                    StatusCode = StatusCodes.Status403Forbidden
                };
                context.ExceptionHandled = true;
                break;
            case DomainException ex:
                context.Result = new RedirectToActionResult("Error", "Home", new { message = ex.Message });
                context.ExceptionHandled = true;
                break;
        }
    }
}