using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Обновление профиля текущего пользователя.
/// </summary>
/// <param name="Dto">Данные профиля.</param>
public sealed record UpdateMyProfileCommand(UpdateMyProfileDto Dto) : IRequest;
