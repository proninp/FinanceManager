using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.API.Extensions;
using FinanceManager.CatalogService.Contracts.DTOs.Categories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ILogger = Serilog.ILogger;

namespace FinanceManager.CatalogService.API.Controllers;

/// <summary>
/// Контроллер для управления категориями.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[SwaggerTag("Контроллер для работы с категориями")]
[Produces("application/json")]
public class CategoryController(ICategoryService categoryService, ILogger logger) : ControllerBase
{
    /// <summary>
    /// Получает категорию по уникальному идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор категории (GUID).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Результат выполнения операции:
    /// - 200 OK с данными категории (<see cref="CategoryDto"/>),
    /// - 400 Bad Request при невалидном идентификаторе,
    /// - 404 Not Found если категория не существует,
    /// - 500 Internal Server Error при внутренних ошибках.
    /// </returns>
    /// <response code="200">Категория найдена и возвращена.</response>
    /// <response code="400">Некорректный формат идентификатора.</response>
    /// <response code="404">Категория с указанным ID не найдена.</response>
    /// <response code="500">Ошибка на сервере.</response>
    /// <example>
    /// Пример запроса:
    /// GET /api/v1/category/3fa85f64-5717-4562-b3fc-2c963f66afa6
    /// </example>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Получение категории по идентификатору",
        Description = "Возвращает категорию по указанному идентификатору")]
    [SwaggerResponse(200, "Категория успешно найдена", typeof(CategoryDto))]
    [SwaggerResponse(400, "Некорректный идентификатор")]
    [SwaggerResponse(404, "Категория не найдена")]
    [SwaggerResponse(500, "Внутренняя ошибка сервера")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        logger.Information("Запрос информации о категории по Id: {CategoryId}", id);
        
        var result = await categoryService.GetByIdAsync(id, cancellationToken);

        return result.ToActionResult(this);
    }
    
    /// <summary>
    /// Получение списка категорий с фильтрацией и пагинацией.
    /// </summary>
    /// <param name="filter">Параметры фильтрации и пагинации.</param>
    /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
    /// <returns>ActionResult со списком категорий или соответствующим статусом ошибки.</returns>
    /// <example>
    /// Пример запроса:
    /// <code>
    /// GET /api/category?ItemsPerPage=10&amp;Page=1&amp;RegistryHolderId=%7BGuid%7D&amp;NameContains=Food
    /// </code>
    /// </example>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получение списка категорий с фильтрацией и пагинацией",
        Description = "Возвращает список категорий с возможностью фильтрации по различным параметрам")]
    [SwaggerResponse(200, "Список категорий успешно получен", typeof(ICollection<CategoryDto>))]
    [SwaggerResponse(400, "Некорректные параметры фильтрации")]
    [SwaggerResponse(500, "Внутренняя ошибка сервера")]
    public async Task<ActionResult<ICollection<CategoryDto>>> Get(
        [FromQuery] CategoryFilterDto filter,
        CancellationToken cancellationToken)
    {
        logger.Information("Запрос списка категорий с фильтрацией: {@Filter}", filter);

        var result = await categoryService.GetPagedAsync(filter, cancellationToken);

        return result.ToActionResult(this);
    }
}