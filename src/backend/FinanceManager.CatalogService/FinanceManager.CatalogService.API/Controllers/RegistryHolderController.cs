using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.API.Extensions;
using FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ILogger = Serilog.ILogger;

namespace FinanceManager.CatalogService.API.Controllers;

/// <summary>
/// Контроллер для работы с пользователями.
/// </summary>
/// <param name="registryHolderService">Сервис для работы с пользователями.</param>
/// <param name="logger">Логгер для записи информации о запросах и ошибках.</param>
[ApiController]
[Route("api/v1/[controller]")]
[SwaggerTag("Контроллер для работы с пользователями")]
[Produces("application/json")]
public class RegistryHolderController(IRegistryHolderService registryHolderService, ILogger logger) : ControllerBase
{
    /// <summary>
    /// Получение пользователя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>ActionResult с данными пользователя или соответствующим статусом ошибки.</returns>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Получение пользователя по идентификатору",
        Description = "Возвращает пользователя по указанному идентификатору")]
    [SwaggerResponse(200, "Пользователь успешно найден", typeof(RegistryHolderDto))]
    [SwaggerResponse(400, "Некорректный идентификатор")]
    [SwaggerResponse(404, "Пользователь не найден")]
    [SwaggerResponse(500, "Внутренняя ошибка сервера")]
    public async Task<ActionResult<RegistryHolderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        logger.Information("Запрос информации о пользователе по Id: {RegistryHolderId}", id);
        
        var result = await registryHolderService.GetByIdAsync(id, cancellationToken);

        return result.ToActionResult(this);
    }
}