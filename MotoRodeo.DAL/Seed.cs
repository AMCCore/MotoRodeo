using DMCorp.Framework.Basics.DAL;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.DAL;

/// <summary>
/// Инициализация стартовых данных. Идемпотентна.
/// </summary>
public static class Seed
{
    /// <summary>
    /// Предопределённый идентификатор учётной записи администратора.
    /// </summary>
    public static Guid AdminAccountId = Guid.Parse(Environment.GetEnvironmentVariable(nameof(AdminAccountId)) ?? throw new InvalidOperationException("AdminAccountId is not set."));
    public static string AdminAccountLogin = Environment.GetEnvironmentVariable(nameof(AdminAccountLogin)) ?? throw new InvalidOperationException("AdminAccountLogin is not set.");
    public static string AdminAccountPass = Environment.GetEnvironmentVariable(nameof(AdminAccountPass)) ?? throw new InvalidOperationException("AdminAccountPass is not set.");

    /// <summary>
    /// Выполняет инициализацию стартовых данных.
    /// </summary>
    /// <param name="uw">Единица работы (Unit of Work).</param>
    /// <param name="adminLogin">Логин администратора.</param>
    /// <param name="adminPassword">Пароль администратора.</param>
    /// <param name="adminName">Отображаемое имя администратора.</param>
    public static void SeedData(this IUnitOfWork uw)
    {
        uw.NotChangeLastUpdateTick = true;

        if (!uw.GetSet<DBAccount>().Any(x => x.Id == AdminAccountId))
        {
            uw.AddEntity(new DBAccount
            {
                Id = AdminAccountId,
                Login = AdminAccountLogin,
                FirstName = "admin",
                LastName = "admin",
                Confirmed = true,
                DateCreated = DateTimeOffset.UtcNow,
            });
        }

        if (!uw.GetSet<DBAccountLogin>().Any(x => x.AccountId == AdminAccountId && x.AccountLoginType == AccountLoginTypeEnum.Login))
        {
            uw.AddEntity(new DBAccountLogin
            {
                AccountId = AdminAccountId,
                AccountLoginType = AccountLoginTypeEnum.Login,
                Login = AdminAccountLogin,
                Password = BCrypt.Net.BCrypt.HashPassword(AdminAccountPass, 11)
            });
        }

        if (!uw.GetSet<DBAccountRight>().Any(x => x.AccountId == AdminAccountId && x.Right == AccountRightEnum.IsAdmin))
        {
            uw.AddEntity(new DBAccountRight
            {
                AccountId = AdminAccountId,
                Right = AccountRightEnum.IsAdmin
            });
        }

        uw.SaveChanges();
    }
}