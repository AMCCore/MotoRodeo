using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик запроса на восстановление пароля.
/// </summary>
public sealed class RequestPasswordResetCommandHandler(IUnitOfWork unitOfWork, IEmailSender emailSender)
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
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var accountLogin = await unitOfWork.Query<DBAccountLogin>()
            .Where(x => x.AccountLoginType == AccountLoginTypeEnum.Login && x.Login == email)
            .Select(x => new { x.AccountId, x.Login })
            .FirstOrDefaultAsync(cancellationToken);

        if (accountLogin == null)
        {
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
    }
}
