using FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Abstractions;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Implementations;

/// <summary>
/// Обработчик исключений типа ArgumentException
/// </summary>
/// <param name="logger">Логгер для записи информации об исключениях</param>
public sealed class ArgumentExceptionHandler(ILogger logger) : IExceptionHandler<ArgumentException>
{
    private readonly ILogger _logger = logger.ForContext<ArgumentExceptionHandler>();

    /// <inheritdoc />
    public ProblemDetails Handle(ArgumentException  exception)
    {
        _logger.Warning("Обработка ArgumentException: {Message}", exception.Message);
        
        return new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Некорректные параметры запроса",
            Detail = exception.Message,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
        };
    }
}