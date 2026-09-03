using DMCorp.Framework.Basics.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.DAL;

namespace MotoRodeo.Web.Controllers;

/// <summary>
/// Контроллер для выполнения миграций БД и сида. Только для операционного развёртывания; не открывать публично.
/// </summary>
[Authorize]
public class MigrateController(IUnitOfWork unitOfWork) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Выполняет миграцию базы данных и инициализацию начальных данных.
    /// </summary>
    /// <returns>Результат выполнения миграции.</returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("/Migrate")]
    public IActionResult MigrateDatabase()
    {
        _unitOfWork.Context.Database.SetCommandTimeout(1000);
        _unitOfWork.Context.Database.Migrate();
        _unitOfWork.SeedData();

        return Ok();
    }
}