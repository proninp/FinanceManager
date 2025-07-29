using FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Abstractions;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Implementations;

public sealed class FallbackExceptionHandler(ILogger logger) : IFallbackExceptionHandler
{
    private readonly ILogger _logger = logger.ForContext<FallbackExceptionHandler>();

    /// <inheritdoc />
    public ProblemDetails Handle(Exception exception)
    {
        _logger.Error(exception, "Необработанное исключение: {ExceptionType}", exception.GetType().Name);

        return new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Внутренняя ошибка сервера",
            Detail = "Произошла непредвиденная ошибка сервера",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
        };
    }
}