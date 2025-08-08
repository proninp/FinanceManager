using System.ComponentModel.DataAnnotations;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.API.Controllers.Filters;
using FinanceManager.CatalogService.API.Extensions;
using FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ILogger = Serilog.ILogger;

namespace FinanceManager.CatalogService.API.Controllers;

/// <summary>
/// Контроллер для работы с владельцами справочников.
/// </summary>
/// <param name="registryHolderService">Сервис для работы с владельцами справочников.</param>
/// <param name="logger">Логгер для записи информации о запросах и ошибках.</param>
[ApiController]
[ServiceFilter(typeof(ModelStateValidationFilter))]
[Route("api/v1/[controller]")]
[SwaggerTag("Контроллер для работы с владельцами справочников")]
[Produces("application/json")]
public class RegistryHolderController(IRegistryHolderService registryHolderService, ILogger logger) : ControllerBase
{
    /// <summary>
    /// Получение пользователя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>ActionResult с данными пользователя или соответствующим статусом ошибки.</returns>
    /// <example>
    /// Пример запроса:
    /// <code>
    /// GET /api/RegistryHolder/{id}
    /// </code>
    /// </example>
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

    /// <summary>
    /// Получение списка владельцев справочников с фильтрацией и пагинацией.
    /// </summary>
    /// <param name="filter">Параметры фильтрации и пагинации.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>ActionResult со списком владельцев справочников или соответствующим статусом ошибки.</returns>
    /// <example>
    /// Пример запроса:
    /// <code>
    /// GET /api/RegistryHolder?role=User&amp;page=1&amp;pageSize=10
    /// </code>
    /// </example>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получение списка владельцев справочников с фильтрацией",
        Description =
            "Возвращает список владельцев справочников с возможностью фильтрации по Telegram ID и роли, а также пагинации")]
    [SwaggerResponse(200, "Список владельцев справочников успешно получен", typeof(ICollection<RegistryHolderDto>))]
    [SwaggerResponse(400, "Некорректные параметры фильтрации")]
    [SwaggerResponse(500, "Внутренняя ошибка сервера")]
    public async Task<ActionResult<ICollection<RegistryHolderDto>>> Get(
        [FromQuery] RegistryHolderFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Запрос списка владельцев справочников с фильтрацией: {@Filter}", filter);

        var result = await registryHolderService.GetPagedAsync(filter, cancellationToken);

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Создает нового владельца справочника.
    /// </summary>
    /// <param name="dto">DTO с данными для создания владельца.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Результат выполнения операции:
    /// <list type="bullet">
    ///     <item><description>201 Created - при успешном создании (возвращает созданный объект <see cref="RegistryHolderDto"/>)</description></item>
    ///     <item><description>400 Bad Request - при некорректных данных</description></item>
    ///     <item><description>409 Conflict - при попытке создания дубликата (по TelegramId)</description></item>
    ///     <item><description>500 Internal Server Error - при внутренних ошибках сервера</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// Логирует:
    /// <list type="bullet">
    ///     <item><description>Факт поступления запроса (с сериализованным DTO)</description></item>
    ///     <item><description>Успешное создание (с ID и ролью нового владельца)</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// Пример запроса:
    /// <code>
    /// POST /api/RegistryHolder
    /// {
    ///     "telegramId": 123456789,
    ///     "role": "User"
    /// }
    /// </code>
    /// </example>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Создание нового владельца справочника",
        Description = "Создаёт нового владельца справочника с указанными параметрами")]
    [SwaggerResponse(201, "Владелец справочника успешно создан", typeof(RegistryHolderDto))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(409, "Владелец справочника с таким TelegramId уже существует")]
    [SwaggerResponse(500, "Внутренняя ошибка сервера")]
    public async Task<ActionResult<RegistryHolderDto>> Create([FromBody, Required] CreateRegistryHolderDto dto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Запрос на создание владельца справочника: {@CreateDto}", dto);

        var result = await registryHolderService.CreateAsync(dto, cancellationToken);

        if (result.IsSuccess)
        {
            logger.Information("Владелец справочника {RegistryHolderId} успешно создан с ролью '{Role}'",
                result.Value.Id, result.Value.Role);
        }

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Обновляет существующего владельца справочника.
    /// </summary>
    /// <param name="dto">DTO с данными для обновления владельца.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Результат выполнения операции:
    /// <list type="bullet">
    ///     <item><description>200 OK - при успешном обновлении (возвращает обновленный объект <see cref="RegistryHolderDto"/>)</description></item>
    ///     <item><description>400 Bad Request - при некорректных данных</description></item>
    ///     <item><description>404 Not Found - если владелец справочника не найден</description></item>
    ///     <item><description>409 Conflict - при попытке обновления с неуникальным TelegramId</description></item>
    ///     <item><description>500 Internal Server Error - при внутренних ошибках сервера</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// Логирует:
    /// <list type="bullet">
    ///     <item><description>Факт поступления запроса (с сериализованным DTO)</description></item>
    ///     <item><description>Успешное обновление (с ID обновленного владельца)</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// Пример запроса:
    /// <code>
    /// PUT /api/RegistryHolder
    /// {
    ///     "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "telegramId": 123456789,
    ///     "role": "Admin"
    /// }
    /// </code>
    /// </example>
    [HttpPut]
    [SwaggerOperation(
        Summary = "Обновление существующего владельца справочника",
        Description = "Обновляет существующего владельца справочника с указанными параметрами")]
    [SwaggerResponse(200, "Владелец справочника успешно обновлен", typeof(RegistryHolderDto))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(404, "Владелец справочника не найден")]
    [SwaggerResponse(409, "Владелец справочника с таким TelegramId уже существует")]
    [SwaggerResponse(500, "Внутренняя ошибка сервера")]
    public async Task<ActionResult<RegistryHolderDto>> Update([FromBody, Required] UpdateRegistryHolderDto dto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Запрос на обновление владельца справочника: {@UpdateDto}", dto);

        var result = await registryHolderService.UpdateAsync(dto, cancellationToken);

        if (result.IsSuccess)
        {
            logger.Information("Владелец справочника {RegistryHolderId} успешно обновлен", dto.Id);
        }

        return result.ToActionResult(this);
    }

    /// <summary>
    /// Удаление владельца справочника по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор владельца справочника.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>
    /// Результат выполнения операции:
    /// <list type="bullet">
    ///     <item><description>200 OK - если владелец успешно удален</description></item>
    ///     <item><description>400 Bad Request - если идентификатор некорректен</description></item>
    ///     <item><description>404 Not Found - если владелец не найден</description></item>
    ///     <item><description>409 Conflict - если владелец не может быть удален, так как используется в других сущностях</description></item>
    ///     <item><description>500 Internal Server Error - при внутренних ошибках сервера</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// Логирует:
    /// <list type="bullet">
    ///     <item><description>Запрос на удаление владельца (с указанным идентификатором)</description></item>
    ///     <item><description>Факт успешного удаления владельца (с ID)</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// Пример запроса:
    /// <code>
    /// DELETE /api/RegistryHolder/{id}
    /// </code>
    /// </example>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Удаление владельца справочника по идентификатору",
        Description = "Удаляет владельца справочника по указанному идентификатору")]
    [SwaggerResponse(200, "Владелец справочника успешно удален")]
    [SwaggerResponse(400, "Некорректный идентификатор")]
    [SwaggerResponse(404, "Владелец справочника не найден")]
    [SwaggerResponse(409, "Владелец справочника не может быть удален, так как используется в других сущностях")]
    [SwaggerResponse(500, "Внутренняя ошибка сервера")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        logger.Information("Запрос на удаление владельца справочника по Id: {RegistryHolderId}", id);

        var result = await registryHolderService.DeleteAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            logger.Information("Владелец справочника {RegistryHolderId} успешно удален", id);
        }

        return result.ToActionResult(this);
    }
}