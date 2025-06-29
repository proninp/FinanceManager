using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.EntityFramework.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.CatalogService.EntityFramework;

/// <summary>
/// Предоставляет методы для регистрации сервисов и настроек базы данных в DI-контейнере.
/// </summary>
public static class Installer
{
    /// <summary>
    /// Регистрирует контекст базы данных и связанные настройки в контейнере зависимостей.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Коллекция сервисов с добавленными зависимостями для работы с базой данных.</returns>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DbSettings>(configuration.GetSection(nameof(DbSettings)));
        services.AddDbContext<IUnitOfWork, DatabaseContext>();
        return services;
    }
}