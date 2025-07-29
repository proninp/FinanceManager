using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Abstractions;

/// <summary>
/// Интерфейс для обработки исключений общего типа (fallback)
/// </summary>
public interface IFallbackExceptionHandler
{
    /// <summary>
    /// Обрабатывает любое исключение как fallback
    /// </summary>
    /// <param name="exception">Исключение для обработки</param>
    /// <returns>Детали проблемы в формате RFC 7807</returns>
    ProblemDetails Handle(Exception exception);
}