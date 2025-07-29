using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Abstractions;

public interface IExceptionHandler<in TException> where TException : Exception
{
    /// <summary>
    /// Обрабатывает исключение и возвращает детали проблемы
    /// </summary>
    /// <param name="exception">Исключение для обработки</param>
    /// <returns>Детали проблемы в формате RFC 7807</returns>
    ProblemDetails Handle(TException exception);
}