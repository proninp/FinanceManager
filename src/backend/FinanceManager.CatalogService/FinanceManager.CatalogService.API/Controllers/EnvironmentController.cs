using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.SystemInfo;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FinanceManager.CatalogService.API.Controllers;

/// <summary>
/// Контроллер информации о сервисе
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[SwaggerTag("Контроллер информации о сервисе")]
[Produces("application/json")]
public sealed class EnvironmentController(ISystemInfoService systemInfoService, Serilog.ILogger logger) : ControllerBase
{
    /// <summary>
    /// Возвращает системную информацию о приложении и окружении
    /// </summary>
    /// <returns>Структурированная информация о системе</returns>
    /// <response code="200">Системная информация успешно получена</response>
    /// <response code="500">Внутренняя ошибка сервера</response>
    [HttpGet("info")]
    [SwaggerOperation(
        Summary = "Получение системной информации",
        Description = "Возвращает подробную информацию о приложении, платформе и окружении выполнения"
    )]
    [ProducesResponseType(typeof(SystemInfoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public ActionResult<SystemInfoResponseDto> GetSystemInfo()
    {
        try
        {
            logger.Information("Запрос системной информации");
            var systemInfoResponse = systemInfoService.GetSystemInfo();
            logger.Information("Системная информация успешно возвращена");
            return Ok(systemInfoResponse);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Ошибка при получении системной информации");
            return Problem(
                title: "Ошибка получения системной информации",
                detail: "Не удалось получить информацию о системе",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    /// <summary>
    /// Проверка доступности сервиса (Health Check)
    /// </summary>
    /// <returns>Статус сервиса</returns>
    /// <response code="200">Сервис доступен</response>
    [HttpGet("health")]
    [SwaggerOperation(
        Summary = "Проверка работоспособности",
        Description = "Простая проверка доступности сервиса"
    )]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        logger.Information("Получен Health check запрос");
        return Ok(new {Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
}