namespace MotoRodeo.BL.Services;

/// <summary>
/// Случайное перемешивание списка участников.
/// </summary>
public interface IParticipantShuffler
{
    /// <summary>
    /// Возвращает новый список с элементами в случайном порядке.
    /// </summary>
    /// <typeparam name="T">Тип элементов.</typeparam>
    /// <param name="items">Исходный список.</param>
    /// <returns>Перемешанная копия списка.</returns>
    IReadOnlyList<T> Shuffle<T>(IReadOnlyList<T> items);
}
