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
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var login = request.Login?.Trim();


        var loginTaken = await unitOfWork.GetSet<DBAccountLogin>()
            .AnyAsync(x => x.Login == login && x.AccountLoginType == AccountLoginTypeEnum.Login, cancellationToken);
        if (loginTaken)
        {
            throw new DomainException("Такой логин уже занят.");
        }

        var account = new DBAccount
        {
            FirstName = firstName,
            LastName = lastName,
            Login = login,
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
