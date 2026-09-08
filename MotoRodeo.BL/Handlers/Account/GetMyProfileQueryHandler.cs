using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Dtos;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик загрузки профиля текущего пользователя.
/// </summary>
public sealed class GetMyProfileQueryHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<GetMyProfileQueryHandler> logger)
    : IRequestHandler<GetMyProfileQuery, MyProfileDto>
{
    /// <inheritdoc />
    public async Task<MyProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        Access.RequireAuthenticated(security);

        logger.LogInformation("Загрузка профиля. AccountId={AccountId}", security.CurrentAccountId);

        var profile = await unitOfWork.Query<DBAccount>()
            .Where(x => x.Id == security.CurrentAccountId)
            .Select(x => new MyProfileDto
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Nickname = x.Login,
                Email = x.AccountLogins
                    .Where(l => l.AccountLoginType == AccountLoginTypeEnum.Login)
                    .Select(l => l.Login)
                    .FirstOrDefault() ?? string.Empty,
                Vehicle = x.Vehicle
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Учётная запись не найдена.");

        return profile;
    }
}