using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик регистрации нового пользователя.
/// </summary>
public sealed class RegisterUserCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<RegisterUserCommand>
{
    /// <summary>
    /// Создаёт неподтверждённую учётную запись и отправляет письмо с ссылкой активации.
    /// </summary>
    /// <param name="request">Данные регистрации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        ArgumentNullException.ThrowIfNull(dto.Email);

        var firstName = dto.FirstName.Trim();
        var lastName = dto.LastName.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();
        var nickname = dto.Nickname?.Trim();
        var vehicle = string.IsNullOrWhiteSpace(dto.Vehicle) ? null : dto.Vehicle.Trim();
        var phoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim();

        logger.LogInformation("Начало регистрации пользователя. Email={Email}", email);

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        if (await unitOfWork.Query<DBAccountLogin>().AnyAsync(x => x.Login == email && x.AccountLoginType == AccountLoginTypeEnum.Login, cancellationToken))
        {
            throw new Exception("Пользователь с такой электронной почтой уже существует.");
        }

        var account = new DBAccount
        {
            FirstName = firstName,
            LastName = lastName,
            Login = nickname,
            Vehicle = vehicle,
            PhoneNumber = phoneNumber,
            Confirmed = false,
            DateCreated = DateTimeOffset.UtcNow
        };
        unitOfWork.AddEntity(account);
        unitOfWork.AddEntity(new DBAccountLogin
        {
            AccountId = account.Id,
            Login = email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password, 11),
            AccountLoginType = AccountLoginTypeEnum.Login
        });

        await unitOfWork.CommitAsync(cancellationToken);

#if !DEBUG

        await emailSender.SendAsync(
            email,
            MailOptions.EmailSubjectRegistration,
            MailOptions.EmailTemplateRegistration.Replace("{link}", dto.ConfirmationLinkFactory(account.Id), StringComparison.Ordinal),
            cancellationToken);

#endif

        logger.LogInformation("Пользователь зарегистрирован. AccountId={AccountId}, Email={Email}", account.Id, email);
    }
}