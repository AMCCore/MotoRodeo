using DMCorp.Framework.Basics.DAL;
using MotoRodeo.DAL.Context;

namespace MotoRodeo.DAL;

/// <summary>
/// Единица работы для управления транзакциями и доступом к данным.
/// </summary>
/// <param name="context">Контекст базы данных.</param>
public sealed class UnitOfWork(MotoRodeoContext context) : BaseUnitOfWork<MotoRodeoContext>(context);