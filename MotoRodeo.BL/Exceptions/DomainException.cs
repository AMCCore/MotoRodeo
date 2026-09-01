namespace MotoRodeo.BL.Exceptions;

/// <summary>
/// Нарушение бизнес-правила.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Создаёт исключение с сообщением о нарушении правила.
    /// </summary>
    /// <param name="message">Описание ошибки для пользователя.</param>
    public DomainException(string message) : base(message)
    {
    }
}
