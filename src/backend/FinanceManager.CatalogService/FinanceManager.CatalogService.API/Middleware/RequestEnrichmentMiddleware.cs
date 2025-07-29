using Serilog.Context;

namespace FinanceManager.CatalogService.API.Middleware;

/// <summary>
/// Middleware для обогащения логов контекстом HTTP-запроса.
/// </summary>
/// <remarks>
/// Добавляет в логи следующие свойства запроса:
/// - TraceId (идентификатор трассировки)
/// - RequestPath (путь запроса)
/// - RequestMethod (HTTP-метод)
/// </remarks>
public sealed class RequestEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Создает экземпляр middleware.
    /// </summary>
    /// <param name="next">Следующий делегат в конвейере запросов.</param>
    public RequestEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    /// <summary>
    /// Обрабатывает HTTP-запрос, добавляя контекст в логи.
    /// </summary>
    /// <param name="httpContext">Контекст HTTP-запроса.</param>
    /// <remarks>
    /// Использует <see cref="LogContext.PushProperty"/> для добавления свойств в логи Serilog.
    /// Контекст автоматически удаляется после выполнения следующего middleware.
    /// </remarks>
    public async Task Invoke(HttpContext httpContext)
    {
        var traceId = httpContext.TraceIdentifier;
        var path = httpContext.Request.Path;
        var method = httpContext.Request.Method;

        using (LogContext.PushProperty("TraceId", traceId))
        using (LogContext.PushProperty("RequestPath", path))
        using (LogContext.PushProperty("RequestMethod", method))
        {
            await _next(httpContext);
        }
    }
}