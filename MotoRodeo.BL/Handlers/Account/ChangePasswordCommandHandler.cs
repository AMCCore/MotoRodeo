using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик смены пароля текущего пользователя.
/// </summary>
public sealed class ChangePasswordCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<ChangePasswordCommandHandler> logger)
    : IRequestHandler<ChangePasswordCommand>
{
    /// <inheritdoc />
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        Access.RequireAuthenticated(security);

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new InvalidOperationException("Новый пароль не может быть пустым.");
        }

        logger.LogInformation("Смена пароля. AccountId={AccountId}", security.CurrentAccountId);

        var accountLogin = await unitOfWork.Query<DBAccountLogin>()
            .SingleOrDefaultAsync(
                x => x.AccountId == security.CurrentAccountId
                     && x.AccountLoginType == AccountLoginTypeEnum.Login,
                cancellationToken)
            ?? throw new KeyNotFoundException("Учётная запись не найдена.");

        if (accountLogin.Password is null || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, accountLogin.Password))
        {
            throw new InvalidOperationException("Неверный текущий пароль.");
        }

        accountLogin.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, 11);
        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        logger.LogInformation("Пароль изменён. AccountId={AccountId}", security.CurrentAccountId);
    }
}