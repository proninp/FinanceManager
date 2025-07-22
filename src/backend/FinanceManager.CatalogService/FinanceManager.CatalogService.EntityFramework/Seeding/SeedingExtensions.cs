using FinanceManager.CatalogService.EntityFramework.Seeding.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinanceManager.CatalogService.EntityFramework.Seeding;

/// <summary>
/// Класс расширений для заполнения базы данных начальными данными
/// </summary>
public static class SeedingExtensions
{
    /// <summary>
    /// Выполняет заполнение базы данных начальными данными через все зарегистрированные IDataSeeder
    /// </summary>
    /// <param name="host">Экземпляр IHost, содержащий конфигурацию приложения и сервисы</param>
    /// <returns>
    /// Возвращает тот же экземпляр IHost для поддержки цепочки вызовов.
    /// Задача завершается после выполнения всех операций заполнения.
    /// </returns>
    public static async Task<IHost> SeedDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync();
        }
        return host;
    }
}