using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик обновления профиля текущего пользователя.
/// </summary>
public sealed class UpdateMyProfileCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<UpdateMyProfileCommandHandler> logger)
    : IRequestHandler<UpdateMyProfileCommand>
{
    /// <inheritdoc />
    public async Task Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
    {
        Access.RequireAuthenticated(security);

        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var nickname = string.IsNullOrWhiteSpace(request.Nickname) ? null : request.Nickname.Trim();
        var vehicle = string.IsNullOrWhiteSpace(request.Vehicle) ? null : request.Vehicle.Trim();

        logger.LogInformation("Обновление профиля. AccountId={AccountId}", security.CurrentAccountId);

        var account = await unitOfWork.Query<DBAccount>()
            .SingleOrDefaultAsync(x => x.Id == security.CurrentAccountId, cancellationToken)
            ?? throw new KeyNotFoundException("Учётная запись не найдена.");

        var duplicate = await unitOfWork.Query<DBAccount>()
            .AnyAsync(
                x => x.Id != account.Id
                     && x.FirstName == firstName
                     && x.LastName == lastName
                     && x.Login == nickname,
                cancellationToken);

        if (duplicate)
        {
            throw new InvalidOperationException("Пользователь с такими именем, фамилией и прозвищем уже существует.");
        }

        account.FirstName = firstName;
        account.LastName = lastName;
        account.Login = nickname;
        account.Vehicle = vehicle;

        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        logger.LogInformation("Профиль обновлён. AccountId={AccountId}", security.CurrentAccountId);
    }
}