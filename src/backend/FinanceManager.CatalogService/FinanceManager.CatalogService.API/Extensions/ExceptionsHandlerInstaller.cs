using FinanceManager.CatalogService.API.Middleware.ExceptionHandlers;
using FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Abstractions;
using FinanceManager.CatalogService.API.Middleware.ExceptionHandlers.Implementations;

namespace FinanceManager.CatalogService.API.Extensions;

/// <summary>
/// Методы расширения для регистрации сервисов обработки исключений
/// </summary>
public static class ExceptionsHandlerInstaller
{
    /// <summary>
    /// Регистрация Exception Handling средств с использованием Switch Expression
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns>Коллекция сервисов для fluent API</returns>
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionHandlerRegistry, ExceptionHandlerRegistry>();
        services.AddSingleton<IFallbackExceptionHandler, FallbackExceptionHandler>();

        services.AddSingleton<IExceptionHandler<ArgumentNullException>, ArgumentNullExceptionHander>();
        services.AddSingleton<IExceptionHandler<ArgumentException>, ArgumentExceptionHandler>();
        services.AddSingleton<IExceptionHandler<InvalidOperationException>, InvalidOperationExceptionHandler>();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions.TryAdd("TraceId", context.HttpContext.TraceIdentifier);
            };
        });
        return services;
    }
}