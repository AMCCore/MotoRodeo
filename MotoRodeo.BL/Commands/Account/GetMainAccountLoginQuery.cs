using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Запрос учётных данных входа по логину.
/// </summary>
/// <param name="Login">Логин пользователя.</param>
/// <returns>Данные для проверки пароля или <c>null</c>, если пользователь не найден.</returns>
public sealed record GetMainAccountLoginQuery(string Login) : IRequest<AccountLoginDto?>;