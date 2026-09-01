using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик регистрации нового пользователя.
/// </summary>
public sealed class RegisterUserCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterUserCommand, Guid>
{
    /// <summary>
    /// Создаёт учётную запись с логином и правом участия.
    /// </summary>
    /// <param name="request">Данные регистрации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Идентификатор созданной учётной записи.</returns>
    /// <exception cref="DomainException">Некорректные данные или логин уже занят.</exception>
    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        var login = request.Login.Trim();
        if (string.IsNullOrWhiteSpace(name) || name.Length > 127)
        {
            throw new DomainException("Укажите имя (до 127 символов).");
        }

        if (string.IsNullOrWhiteSpace(login) || login.Length > 127)
        {
            throw new DomainException("Укажите логин (до 127 символов).");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            throw new DomainException("Пароль должен быть не короче 6 символов.");
        }

        var loginTaken = await unitOfWork.GetSet<DBAccountLogin>()
            .AnyAsync(x => x.Login == login && x.AccountLoginType == AccountLoginTypeEnum.Login, cancellationToken);
        if (loginTaken)
        {
            throw new DomainException("Такой логин уже занят.");
        }

        var account = new DBAccount
        {
            Name = name,
            Confirmed = true,
            DateCreated = DateTimeOffset.UtcNow
        };
        unitOfWork.AddEntity(account);
        unitOfWork.AddEntity(new DBAccountLogin
        {
            AccountId = account.Id,
            Login = login,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password, 11),
            AccountLoginType = AccountLoginTypeEnum.Login
        });
        unitOfWork.AddEntity(new DBAccountRight
        {
            AccountId = account.Id,
            Right = AccountRightEnum.CanParticipate
        });
        await unitOfWork.SaveChangesAsync(token: cancellationToken);
        return account.Id;
    }
}
