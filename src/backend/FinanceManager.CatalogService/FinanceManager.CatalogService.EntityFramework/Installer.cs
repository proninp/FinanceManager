using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;

namespace FinanceManager.CatalogService.EntityFramework;

/// <summary>
/// Предоставляет методы для регистрации сервисов и настроек базы данных в DI-контейнере.
/// </summary>
public static class Installer
{
    /// <summary>
    /// Регистрирует контекст базы данных и связанные настройки в контейнере зависимостей.
    /// Позволяет опционально включить логирование чувствительных данных (EnableSensitiveDataLogging).
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <param name="enableSensitiveLogging">Включить логирование чувствительных данных (по умолчанию false).</param>
    /// <returns>Коллекция сервисов с добавленными зависимостями для работы с базой данных.</returns>
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration,
        bool enableSensitiveLogging = false)
    {
        services.Configure<FmcsDbSettings>(configuration.GetSection(nameof(FmcsDbSettings)));
        services.AddDbContext<IUnitOfWork, DatabaseContext>((provider, options) =>
        {
            var dbSettings = provider.GetRequiredService<IOptions<FmcsDbSettings>>().Value;
            options.UseNpgsql(dbSettings.GetConnectionString());

            if (enableSensitiveLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }

    /// <summary>
    /// Применяет все ожидающие миграции Entity Framework Core при запуске приложения.
    /// </summary>
    /// <param name="application">Экземпляр <see cref="WebApplication"/>, предоставляющий доступ к сервисам и жизненному циклу приложения.</param>
    /// <returns>
    /// Задача, представляющая асинхронную операцию применения миграций.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если <see cref="DatabaseContext"/> не зарегистрирован в DI-контейнере.
    /// </exception>ы
    public static async Task UseMigrationAsync(this WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        await dbContext.Database.MigrateAsync(application.Lifetime.ApplicationStopping);
    }
}