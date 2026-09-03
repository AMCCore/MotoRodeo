using Microsoft.EntityFrameworkCore;

namespace MotoRodeo.DAL.Extensions;

/// <summary>
/// Расширения для конфигурирования поведения контекста EF Core.
/// </summary>
public static class ContextExtension
{
    /// <summary>
    /// Отключает каскадное удаление для всех внешних ключей, где оно включено.
    /// </summary>
    /// <param name="modelBuilder">Построитель модели, к которому применяется изменение.</param>
    public static void DisableCascadeDeleteConvention(this ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}