using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик подтверждения восстановления пароля.
/// </summary>
public sealed class ConfirmPasswordResetCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    ILogger<ConfirmPasswordResetCommandHandler> logger)
    : IRequestHandler<ConfirmPasswordResetCommand>
{
    private static readonly TimeSpan ResetLinkLifetime = TimeSpan.FromHours(24);

    /// <summary>
    /// Проверяет срок действия запроса, генерирует новый пароль и отправляет его на почту.
    /// </summary>
    /// <param name="request">Идентификатор запроса восстановления.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task Handle(ConfirmPasswordResetCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Начало подтверждения восстановления пароля. ResetRequestId={ResetRequestId}", request.ResetRequestId);

        var resetRequest = await unitOfWork.Query<DBPasswordResetRequest>().FirstOrDefaultAsync(x => x.Id == request.ResetRequestId, cancellationToken) ?? throw new Exception("Ссылка восстановления пароля недействительна.");

        if (resetRequest.IsUsed)
        {
            throw new Exception("Ссылка восстановления пароля уже была использована.");
        }

        if (DateTimeOffset.UtcNow - resetRequest.DateCreated > ResetLinkLifetime)
        {
            throw new Exception("Срок действия ссылки восстановления пароля истёк.");
        }

        var accountLogin = await unitOfWork.Query<DBAccountLogin>()
            .FirstOrDefaultAsync(x => x.AccountId == resetRequest.AccountId && x.AccountLoginType == AccountLoginTypeEnum.Login, cancellationToken)
            ?? throw new Exception("Учётная запись для восстановления не найдена.");

        var newPassword = PasswordGenerator.Generate();
        accountLogin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword, 11);
        resetRequest.IsUsed = true;
        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        var body = MailOptions.EmailTemplateNewPassword.Replace("{password}", newPassword, StringComparison.Ordinal);
        await emailSender.SendAsync(accountLogin.Login, MailOptions.EmailSubjectNewPassword, body, cancellationToken);

        logger.LogInformation(
            "Пароль сброшен. AccountId={AccountId}, ResetRequestId={ResetRequestId}",
            resetRequest.AccountId,
            resetRequest.Id);
    }
}