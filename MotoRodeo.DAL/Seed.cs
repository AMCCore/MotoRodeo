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
    public static readonly Guid AdminAccountId = Guid.Parse("11111111-1111-4111-8111-111111111111");

    /// <summary>
    /// Выполняет инициализацию стартовых данных.
    /// </summary>
    /// <param name="uw">Единица работы (Unit of Work).</param>
    /// <param name="adminLogin">Логин администратора.</param>
    /// <param name="adminPassword">Пароль администратора.</param>
    /// <param name="adminName">Отображаемое имя администратора.</param>
    public static void SeedData(this IUnitOfWork uw, string adminLogin, string adminPassword, string adminName)
    {
        uw.NotChangeLastUpdateTick = true;

        if (!uw.GetSet<DBAccount>().Any(x => x.Id == AdminAccountId))
        {
            uw.AddEntity(new DBAccount
            {
                Id = AdminAccountId,
                Name = adminName,
                Confirmed = true,
                DateCreated = DateTimeOffset.UtcNow,
            });
        }

        if (!uw.GetSet<DBAccountLogin>().Any(x => x.AccountId == AdminAccountId && x.AccountLoginType == AccountLoginTypeEnum.Login))
        {
            if (!string.IsNullOrEmpty(adminPassword) && !string.IsNullOrEmpty(adminLogin))
            {
                uw.AddEntity(new DBAccountLogin
                {
                    AccountId = AdminAccountId,
                    AccountLoginType = AccountLoginTypeEnum.Login,
                    Login = adminLogin,
                    Password = BCrypt.Net.BCrypt.HashPassword(adminPassword, 11)
                });
            }
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