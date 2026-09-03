using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик регистрации нового пользователя.
/// </summary>
public sealed class RegisterUserCommandHandler(IUnitOfWork unitOfWork, IEmailSender emailSender)
    : IRequestHandler<RegisterUserCommand>
{
    /// <summary>
    /// Создаёт неподтверждённую учётную запись и отправляет письмо с ссылкой активации.
    /// </summary>
    /// <param name="request">Данные регистрации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <exception cref="DomainException">Некорректные данные или email уже занят.</exception>
    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var nickname = string.IsNullOrWhiteSpace(request.Nickname) ? null : request.Nickname.Trim();
        var email = NormalizeEmail(request.Email);

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new Exception("Укажите адрес электронной почты.");
        }

        var emailTaken = await unitOfWork.GetSet<DBAccountLogin>()
            .AnyAsync(x => x.Login == email && x.AccountLoginType == AccountLoginTypeEnum.Login, cancellationToken);
        if (emailTaken)
        {
            throw new Exception("Пользователь с такой электронной почтой уже существует.");
        }

        var account = new DBAccount
        {
            FirstName = firstName,
            LastName = lastName,
            Login = nickname,
            Confirmed = false,
            DateCreated = DateTimeOffset.UtcNow
        };
        unitOfWork.AddEntity(account);
        unitOfWork.AddEntity(new DBAccountLogin
        {
            AccountId = account.Id,
            Login = email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password, 11),
            AccountLoginType = AccountLoginTypeEnum.Login
        });
        unitOfWork.AddEntity(new DBAccountRight
        {
            AccountId = account.Id,
            Right = AccountRightEnum.CanParticipate
        });
        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        var link = request.ConfirmationLinkFactory(account.Id);
        var body = MailOptions.EmailTemplateRegistration.Replace("{link}", link, StringComparison.Ordinal);
        await emailSender.SendAsync(email, MailOptions.EmailSubjectRegistration, body, cancellationToken);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
