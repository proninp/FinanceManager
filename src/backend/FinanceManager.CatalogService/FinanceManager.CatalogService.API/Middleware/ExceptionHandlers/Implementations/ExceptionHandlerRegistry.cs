using System.Collections.Concurrent;
using FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Implementations;

/// <summary>
/// Реестр обработчиков исключений на основе switch expression
/// </summary>
/// <param name="serviceProvider">Провайдер сервисов для разрешения зависимостей</param>
/// <param name="fallbackHandler">Fallback обработчик для неопознанных исключений</param>
public sealed class ExceptionHandlerRegistry(
    IServiceProvider serviceProvider,
    IFallbackExceptionHandler fallbackHandler) : IExceptionHandlerRegistry
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IFallbackExceptionHandler _fallbackHandler = fallbackHandler;
    
    /// <inheritdoc />
    public ProblemDetails HandleException(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return exception switch
        {
            ArgumentNullException argumentNullException =>
                GetHandler<ArgumentNullException>().Handle(argumentNullException),
            
            ArgumentException argumentException => 
                GetHandler<ArgumentException>().Handle(argumentException),
                
            InvalidOperationException invalidOperationException => 
                GetHandler<InvalidOperationException>().Handle(invalidOperationException),
            
            _ => _fallbackHandler.Handle(exception)
        };
    }
    
    /// <summary>
    /// Получает типизированный обработчик из DI контейнера
    /// </summary>
    /// <typeparam name="TException">Тип исключения</typeparam>
    /// <returns>Обработчик исключения</returns>
    private IExceptionHandler<TException> GetHandler<TException>() where TException : Exception
    {
        return _serviceProvider.GetRequiredService<IExceptionHandler<TException>>();
    }
}