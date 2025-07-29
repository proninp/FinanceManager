using FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Abstractions;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Implementations;

/// <summary>
/// Обработчик исключений типа ArgumentNullException
/// </summary>
/// <param name="logger">Логгер для записи информации об исключениях</param>
public sealed class ArgumentNullExceptionHander(ILogger logger) : IExceptionHandler<ArgumentNullException>
{
    private readonly ILogger _logger = logger.ForContext<ArgumentNullExceptionHander>();
    
    /// <inheritdoc />
    public ProblemDetails Handle(ArgumentNullException exception)
    {
        _logger.Warning("Передан null параметр {ParameterName}: {Message}", 
            exception.ParamName ?? "unknown", exception.Message);
        
        return new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Отсутствует обязательный параметр",
            Detail = $"Параметр '{exception.ParamName ?? "unknown"}' не может быть null",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            Extensions = 
            {
                ["parameterName"] = exception.ParamName ?? "unknown"
            }
        };
    }
}