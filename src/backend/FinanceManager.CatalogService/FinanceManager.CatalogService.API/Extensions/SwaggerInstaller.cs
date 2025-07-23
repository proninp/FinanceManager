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
            var openApiInfo = new OpenApiInfo
            {
                Title = "Finance Manager Catalog Service API",
                Version = "v1",
                Description = "API для обработки запросов на получение информации о сущностях каталогов через REST",
                Contact = new OpenApiContact
                {
                    Name = "Команда DotNetRunners",
                    Url = new Uri("https://github.com/proninp/FinanceManager")
                }
            };
            
            options.SwaggerDoc("v1", openApiInfo);
            
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