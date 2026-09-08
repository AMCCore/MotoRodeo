using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Запрос профиля текущего пользователя.
/// </summary>
public sealed record GetMyProfileQuery : IRequest<MyProfileDto>;