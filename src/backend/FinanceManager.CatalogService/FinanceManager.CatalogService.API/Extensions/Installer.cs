using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Repositories.Implementations;
using Serilog;

namespace FinanceManager.CatalogService.API.Extensions;

/// <summary>
/// Класс-установщик для регистрации инфраструктурных компонентов приложения.
/// </summary>
public static class Installer
{
    /// <summary>
    /// Добавляет и настраивает логирование Serilog в приложение.
    /// </summary>
    /// <param name="hostBuilder">Строитель хоста приложения.</param>
    /// <param name="configuration">Конфигурация приложения, содержащая настройки логирования.</param>
    /// <returns>Обновлённый <see cref="IHostBuilder"/> с настроенным логированием.</returns>
    public static IHostBuilder AddLogging(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        return hostBuilder.UseSerilog((context, loggerConfig) =>
            loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId());
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddScoped<ICurrencyRepository, CurrencyRepository>()
            .AddScoped<ICountryRepository, CountryRepository>()
            .AddScoped<IBankRepository, BankRepository>()
            .AddScoped<IAccountRepository, AccountRepository>()
            .AddScoped<IAccountTypeRepository, AccountTypeRepository>()
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<IExchangeRateRepository, ExchangeRateRepository>()
            .AddScoped<IRegistryHolderRepository, RegistryHolderRepository>();
        return services;
    }
}