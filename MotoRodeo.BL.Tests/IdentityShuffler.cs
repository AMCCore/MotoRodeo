using MotoRodeo.BL.Services;

namespace MotoRodeo.BL.Tests;

public sealed class IdentityShuffler : IParticipantShuffler
{
    public IReadOnlyList<T> Shuffle<T>(IReadOnlyList<T> items) => items.ToList();
}
