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
/// Обработчик запроса на восстановление пароля.
/// </summary>
public sealed class RequestPasswordResetCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    ILogger<RequestPasswordResetCommandHandler> logger)
    : IRequestHandler<RequestPasswordResetCommand>
{
    /// <summary>
    /// Создаёт запись о запросе восстановления и отправляет ссылку, если учётная запись найдена.
    /// </summary>
    /// <param name="request">Email и фабрика ссылки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        logger.LogInformation("Начало запроса восстановления пароля. Email={Email}", email);

        if (string.IsNullOrWhiteSpace(email))
        {
            logger.LogDebug("Запрос восстановления пароля пропущен: пустой email.");
            return;
        }

        var accountLogin = await unitOfWork.Query<DBAccountLogin>()
            .Where(x => x.AccountLoginType == AccountLoginTypeEnum.Login
                        && x.Login == email
                        && !x.Account.IsBlocked)
            .Select(x => new { x.AccountId, x.Login })
            .FirstOrDefaultAsync(cancellationToken);

        if (accountLogin == null)
        {
            logger.LogDebug("Запрос восстановления пароля: учётная запись не найдена или заблокирована. Email={Email}", email);
            return;
        }

        var resetRequest = new DBPasswordResetRequest
        {
            AccountId = accountLogin.AccountId,
            DateCreated = DateTimeOffset.UtcNow
        };
        unitOfWork.AddEntity(resetRequest);
        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        var link = request.ResetLinkFactory(resetRequest.Id);
        var body = MailOptions.EmailTemplatePasswordReset.Replace("{link}", link, StringComparison.Ordinal);
        await emailSender.SendAsync(accountLogin.Login, MailOptions.EmailSubjectPasswordReset, body, cancellationToken);

        logger.LogInformation(
            "Письмо восстановления пароля отправлено. AccountId={AccountId}, ResetRequestId={ResetRequestId}",
            accountLogin.AccountId,
            resetRequest.Id);
    }
}