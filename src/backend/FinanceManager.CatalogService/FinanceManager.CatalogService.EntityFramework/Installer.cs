using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.EntityFramework.Options;
using FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;
using FinanceManager.CatalogService.EntityFramework.Seeding.Data;
using FinanceManager.CatalogService.EntityFramework.Seeding.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

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
    /// Добавляет сервисы для заполнения базы данных начальными данными (сидирование)
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns>Коллекция сервисов с зарегистрированными сидерами</returns>
    /// <remarks>
    /// Регистрирует реализации IDataSeeder для заполнения:
    /// - Стран (CountrySeeder)
    /// - Банков (BankSeeder)
    /// - Валют (CurrencySeeder)
    /// </remarks>
    public static IServiceCollection AddSeeding(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(ISeedingEntitiesProducer<>), typeof(SeedingEntitiesFileProducer<>))
            .AddScoped<IDataSeeder, CountryFileSeeder>()
            .AddScoped<IDataSeeder, BankFileSeeder>()
            .AddScoped<IDataSeeder, CurrencyFileSeeder>();
        return services;
    }

    /// <summary>
    /// Применяет все ожидающие миграции базы данных для контекста DatabaseContext
    /// </summary>
    /// <param name="host">Экземпляр IHost, содержащий конфигурацию приложения и сервисы</param>
    /// <returns>
    /// Возвращает тот же экземпляр IHost для поддержки цепочки вызовов.
    /// Задача завершается после применения всех миграций.
    /// </returns>
    public static async Task<IHost> UseMigrationAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        await dbContext.Database.MigrateAsync();
        return host;
    }
}