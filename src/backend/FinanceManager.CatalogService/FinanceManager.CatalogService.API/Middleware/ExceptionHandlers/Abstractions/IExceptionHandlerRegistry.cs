using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Abstractions;

/// <summary>
/// Регистр обработчиков исключений для определения подходящего обработчика
/// </summary>
public interface IExceptionHandlerRegistry
{
    /// <summary>
    /// Получает подходящий обработчик для указанного исключения
    /// </summary>
    /// <param name="exception">Исключение для обработки</param>
    /// <returns>Детали проблемы в формате RFC 7807</returns>
    ProblemDetails HandleException(Exception exception); 
}