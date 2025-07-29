using System.Net;
using FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace FinanceManager.CatalogService.API.Middleware.ExceptionHandlers;

/// <summary>
/// Глобальный обработчик исключений для ASP.NET Core приложения
/// </summary>
/// <param name="exceptionHandlerRegistry">Реестр обработчиков исключений</param>
/// <param name="logger">Логгер для записи информации об исключениях</param>
public sealed class GlobalExceptionHandler(
    IExceptionHandlerRegistry exceptionHandlerRegistry,
    ILogger logger) : IExceptionHandler
{
    private readonly IExceptionHandlerRegistry _exceptionHandlerRegistry = exceptionHandlerRegistry;
    private readonly ILogger _logger = logger.ForContext<GlobalExceptionHandler>();

    /// <summary>
    /// Обрабатывает исключение асинхронно
    /// </summary>
    /// <param name="httpContext">HTTP контекст запроса</param>
    /// <param name="exception">Исключение для обработки</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если исключение было обработано успешно</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        try
        {
            var problemDetails = _exceptionHandlerRegistry.HandleException(exception);
            
            var statusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
            
            LogException(exception, statusCode);
            AddCorrelationId(httpContext, problemDetails);
            
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";
            
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            
            return true;
        }
        catch (Exception handlerException)
        {
            _logger.Fatal(handlerException, 
                "Критическая ошибка в обработчике исключений при обработке {OriginalException}", 
                exception.GetType().Name);
            
            return false;
        }
    }
    
    /// <summary>
    /// Логирует исключение с соответствующим уровнем в зависимости от HTTP статус-кода
    /// </summary>
    /// <param name="exception">Исключение для логирования</param>
    /// <param name="statusCode">HTTP статус-код</param>
    private void LogException(Exception exception, int statusCode)
    {
        const string messageTemplate = "Обработано исключение: {ExceptionType} - {Message} | StatusCode: {StatusCode}";
        var exceptionType = exception.GetType().Name;
        var message = exception.Message;

        switch (statusCode)
        {
            case >= 500:
                _logger.Error(exception, messageTemplate, exceptionType, message, statusCode);
                break;
            case >= 400:
                _logger.Warning(messageTemplate, exceptionType, message, statusCode);
                break;
            default:
                _logger.Information(messageTemplate, exceptionType, message, statusCode);
                break;
        }
    }
    
    /// <summary>
    /// Добавляет идентификатор корреляции в детали проблемы для трассировки
    /// </summary>
    /// <param name="httpContext">HTTP контекст</param>
    /// <param name="problemDetails">Детали проблемы</param>
    private static void AddCorrelationId(HttpContext httpContext, ProblemDetails problemDetails)
    {
        var traceId = httpContext.TraceIdentifier;
        
        problemDetails.Extensions.TryAdd("TraceId", traceId);
        problemDetails.Instance = httpContext.Request.Path;
    }
}