using FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Abstractions;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Implementations;

/// <summary>
/// Обработчик исключений типа InvalidOperationException
/// </summary>
/// <param name="logger">Логгер для записи информации об исключениях</param>
public sealed class InvalidOperationExceptionHandler(ILogger logger) : IExceptionHandler<InvalidOperationException>
{
    private readonly ILogger _logger = logger.ForContext<InvalidOperationExceptionHandler>();

    /// <inheritdoc />
    public ProblemDetails Handle(InvalidOperationException exception)
    {
        _logger.Warning("Обработка InvalidOperationException: {Message}", exception.Message);
        
        return new ProblemDetails
        {
            Status = StatusCodes.Status422UnprocessableEntity,
            Title = "Операция не может быть выполнена",
            Detail = exception.Message,
            Type = "https://datatracker.ietf.org/doc/html/rfc4918#section-11.2"
        };
    }
}