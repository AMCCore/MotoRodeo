using System.Linq.Expressions;
using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Extensions;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;
using MotoRodeo.DAL.Extensions;

namespace MotoRodeo.DAL.Context;

/// <summary>
/// Основной контекст базы данных МотоРодео.
/// </summary>
public class MotoRodeoContext(DbContextOptions<MotoRodeoContext> options) : DbContext(options)
{
    /// <summary>
    /// Конфигурация сущностей и соглашений модели.
    /// </summary>
    /// <param name="modelBuilder">Построитель модели EF Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DBAccount>().HasIndex(a => a.Name);

        modelBuilder.Entity<DBAccountRight>().Property(d => d.Right)
            .HasConversion(new GuidEnumConverterExtension<AccountRightEnum>());
        modelBuilder.Entity<DBAccountLogin>().Property(d => d.AccountLoginType)
            .HasConversion(new GuidEnumConverterExtension<AccountLoginTypeEnum>());
        modelBuilder.Entity<DBEvent>().Property(d => d.Status)
            .HasConversion(new GuidEnumConverterExtension<EventStatusEnum>());

        modelBuilder.DisableCascadeDeleteConvention();
        modelBuilder.UseIdentityColumns();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Select(x => x.ClrType)
                     .Where(x => typeof(ISoftDeleteEntity).IsAssignableFrom(x)))
        {
            var parameter = Expression.Parameter(entityType, "e");
            var body = Expression.Equal(
                Expression.Call(typeof(EF), nameof(EF.Property), [typeof(bool)], parameter,
                    Expression.Constant("IsDeleted")),
                Expression.Constant(false));
            modelBuilder.Entity(entityType).HasQueryFilter(Expression.Lambda(body, parameter));
        }
    }

    /// <summary>
    /// Таблица учетных записей пользователей.
    /// </summary>
    public DbSet<DBAccount> DBAccounts { get; set; } = null!;

    /// <summary>
    /// Таблица данных для входа пользователей.
    /// </summary>
    public DbSet<DBAccountLogin> DBAccountLogins { get; set; } = null!;

    /// <summary>
    /// Таблица прав пользователей.
    /// </summary>
    public DbSet<DBAccountRight> DBAccountRights { get; set; } = null!;

    /// <summary>
    /// Таблица событий МотоРодео.
    /// </summary>
    public DbSet<DBEvent> DBEvents { get; set; } = null!;

    /// <summary>
    /// Таблица участников событий.
    /// </summary>
    public DbSet<DBEventParticipant> DBEventParticipants { get; set; } = null!;

    /// <summary>
    /// Таблица судей событий.
    /// </summary>
    public DbSet<DBEventJudge> DBEventJudges { get; set; } = null!;

    /// <summary>
    /// Таблица групп участников событий.
    /// </summary>
    public DbSet<DBEventGroup> DBEventGroups { get; set; } = null!;

    /// <summary>
    /// Таблица участников групп.
    /// </summary>
    public DbSet<DBGroupMember> DBGroupMembers { get; set; } = null!;

    /// <summary>
    /// Таблица заездов.
    /// </summary>
    public DbSet<DBHeat> DBHeats { get; set; } = null!;
}