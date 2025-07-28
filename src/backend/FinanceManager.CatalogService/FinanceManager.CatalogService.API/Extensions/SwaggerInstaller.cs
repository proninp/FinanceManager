using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace FinanceManager.CatalogService.API.Extensions;

/// <summary>
/// Добавляет и настраивает Swagger для API.
/// </summary>
/// <param name="services">Коллекция сервисов DI.</param>
/// <param name="configuration">Конфигурация приложения (раздел OpenApiInfo).</param>
/// <returns>Коллекция сервисов для цепочки вызовов.</returns>
public static class SwaggerInstaller
{
    /// <summary>
    /// Добавляет и настраивает Swagger для API.
    /// </summary>
    /// <param name="services">Коллекция сервисов DI.</param>
    /// <param name="configuration">Конфигурация приложения (раздел OpenApiInfo).</param>
    /// <returns>Коллекция сервисов для цепочки вызовов.</returns>
    /// <remarks>
    /// Конфигурация Swagger включает:
    /// - Загрузку метаданных из раздела <see cref="OpenApiInfo"/> в конфигурации,
    /// - XML-документацию из файла сборки,
    /// - Поддержку аннотаций Swagger (например, <see cref="SwaggerOperationAttribute"/>).
    /// </remarks>
    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", configuration.GetSection(nameof(OpenApiInfo)).Get<OpenApiInfo>());

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.EnableAnnotations();
        });
        return services;
    }
}