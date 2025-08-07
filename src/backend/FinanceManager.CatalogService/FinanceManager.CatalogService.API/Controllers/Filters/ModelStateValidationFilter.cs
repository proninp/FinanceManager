using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ILogger = Serilog.ILogger;

namespace FinanceManager.CatalogService.API.Controllers.Filters;

/// <summary>
/// Фильтр действий для автоматической валидации ModelState.
/// </summary>
/// <remarks>
/// Проверяет состояние ModelState перед выполнением действия контроллера.
/// Если обнаружены ошибки валидации:
/// - Логирует предупреждение с деталями ошибок
/// - Возвращает HTTP 400 с перечнем ошибок
/// </remarks>
public sealed class ModelStateValidationFilter(ILogger logger) : ActionFilterAttribute
{
    private readonly ILogger _logger = logger;
    
    /// <summary>
    /// Выполняет валидацию ModelState перед вызовом действия контроллера.
    /// </summary>
    /// <param name="context">Контекст выполнения действия.</param>
    /// <remarks>
    /// При обнаружении ошибок:
    /// 1. Логирует информацию в формате: "Ошибки валидации в {Controller}.{Action}: {Errors}"
    /// 2. Прерывает выполнение, возвращая BadRequest (400) с ошибками валидации
    /// </remarks>
    /// <example>
    /// Пример ответа при ошибках:
    /// <code>
    /// {
    ///     "field1": ["Поле обязательно для заполнения"],
    ///     "field2": ["Значение должно быть числом"]
    /// }
    /// </code>
    /// </example>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;
        
        var controllerName = context.RouteData.Values["controller"];
        var actionName = context.RouteData.Values["action"];
        
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
            
        _logger.Warning("Ошибки валидации в {Controller}.{Action}: {Errors}",
            controllerName, actionName, errors);
            
        context.Result = new BadRequestObjectResult(context.ModelState);
    }
}