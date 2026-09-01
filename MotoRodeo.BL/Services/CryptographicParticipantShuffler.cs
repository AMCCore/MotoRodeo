namespace MotoRodeo.BL.Services;

/// <summary>
/// Перемешивание на криптографически стойком генераторе случайных чисел.
/// </summary>
public sealed class CryptographicParticipantShuffler : IParticipantShuffler
{
    /// <inheritdoc />
    public IReadOnlyList<T> Shuffle<T>(IReadOnlyList<T> items)
    {
        var copy = items.ToList();
        for (var i = copy.Count - 1; i > 0; i--)
        {
            var j = System.Security.Cryptography.RandomNumberGenerator.GetInt32(i + 1);
            (copy[i], copy[j]) = (copy[j], copy[i]);
        }

        return copy;
    }
}
