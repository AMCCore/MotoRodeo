using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MotoRodeo.Web.Filters;

/// <summary>
/// Проверка заголовка X-Api-Key для внешнего API подтверждения участия.
/// Схему можно заменить позже, не меняя команды BL.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class EventParticipationApiKeyAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// Имя переменной окружения с секретом.
    /// </summary>
    public const string EnvKeyName = "EventParticipationApiKey";

    /// <summary>
    /// Имя HTTP-заголовка.
    /// </summary>
    public const string HeaderName = "X-Api-Key";

    /// <inheritdoc />
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var expected = Environment.GetEnvironmentVariable(EnvKeyName);
        if (string.IsNullOrWhiteSpace(expected))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var provided)
            || !FixedTimeEquals(provided.ToString(), expected))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        var ba = System.Text.Encoding.UTF8.GetBytes(a);
        var bb = System.Text.Encoding.UTF8.GetBytes(b);
        if (ba.Length != bb.Length)
        {
            // всё равно сравниваем, чтобы не раскрывать длину по времени слишком явно
            System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(ba, ba);
            return false;
        }

        return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(ba, bb);
    }
}
