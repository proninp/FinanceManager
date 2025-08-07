using System.Reflection;
using System.Text.Json.Serialization;
using FinanceManager.CatalogService.API.Controllers.Filters;
using Microsoft.OpenApi.Models;

namespace FinanceManager.CatalogService.API.Extensions;

/// <summary>
/// Регистрирует Swagger, настраивает контроллеры, фильтры и общие настройки Web API.
/// </summary>
public static class WebApiConfigurationExtensions
{
    /// <summary>
    /// Добавляет базовую конфигурацию для Web API.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Коллекция сервисов для цепочки вызовов.</returns>
    public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services)
    {
        services.AddScoped<ModelStateValidationFilter>();
        
        services
            .AddControllers()
            .AddJsonOptions(options => 
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        return services;
    }
    
    /// <summary>
    /// Добавляет и настраивает Swagger для API.
    /// </summary>
    /// <param name="services">Коллекция сервисов DI.</param>
    /// <param name="configuration">Конфигурация приложения (раздел OpenApiInfo).</param>
    /// <returns>Коллекция сервисов для цепочки вызовов.</returns>
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