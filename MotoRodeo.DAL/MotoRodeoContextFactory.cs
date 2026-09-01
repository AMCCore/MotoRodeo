using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MotoRodeo.DAL.Context;

namespace MotoRodeo.DAL;

/// <summary>
/// Фабрика контекста для миграций EF Core в design-time.
/// </summary>
public sealed class MotoRodeoContextFactory : IDesignTimeDbContextFactory<MotoRodeoContext>
{
    /// <summary>
    /// Создаёт экземпляр контекста базы данных.
    /// </summary>
    /// <param name="args">Аргументы командной строки (не используются).</param>
    /// <returns>Настроенный контекст MotoRodeoContext.</returns>
    public MotoRodeoContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MotoRodeoContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=motorodeo;Username=postgres;Password=postgres")
            .Options;
        return new MotoRodeoContext(options);
    }
}