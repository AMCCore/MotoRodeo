using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик подтверждения восстановления пароля.
/// </summary>
public sealed class ConfirmPasswordResetCommandHandler(IUnitOfWork unitOfWork, IEmailSender emailSender)
    : IRequestHandler<ConfirmPasswordResetCommand>
{
    private static readonly TimeSpan ResetLinkLifetime = TimeSpan.FromHours(24);

    /// <summary>
    /// Проверяет срок действия запроса, генерирует новый пароль и отправляет его на почту.
    /// </summary>
    /// <param name="request">Идентификатор запроса восстановления.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <exception cref="DomainException">Запрос не найден, просрочен или уже использован.</exception>
    public async Task Handle(ConfirmPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var resetRequest = await unitOfWork.GetSet<DBPasswordResetRequest>()
            .FirstOrDefaultAsync(x => x.Id == request.ResetRequestId, cancellationToken);

        if (resetRequest == null)
        {
            throw new DomainException("Ссылка восстановления пароля недействительна.");
        }

        if (resetRequest.IsUsed)
        {
            throw new DomainException("Ссылка восстановления пароля уже была использована.");
        }

        if (DateTimeOffset.UtcNow - resetRequest.DateCreated > ResetLinkLifetime)
        {
            throw new DomainException("Срок действия ссылки восстановления пароля истёк.");
        }

        var accountLogin = await unitOfWork.GetSet<DBAccountLogin>()
            .FirstOrDefaultAsync(
                x => x.AccountId == resetRequest.AccountId && x.AccountLoginType == AccountLoginTypeEnum.Login,
                cancellationToken);

        if (accountLogin == null)
        {
            throw new DomainException("Учётная запись для восстановления не найдена.");
        }

        var newPassword = PasswordGenerator.Generate();
        accountLogin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword, 11);
        resetRequest.IsUsed = true;
        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        var body = MailOptions.EmailTemplateNewPassword.Replace("{password}", newPassword, StringComparison.Ordinal);
        await emailSender.SendAsync(accountLogin.Login, MailOptions.EmailSubjectNewPassword, body, cancellationToken);
    }
}
