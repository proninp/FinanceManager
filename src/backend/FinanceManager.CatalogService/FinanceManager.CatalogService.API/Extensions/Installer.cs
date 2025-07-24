using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Implementations.Errors;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FinanceManager.CatalogService.Implementations.Services;
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

    /// <summary>
    /// Регистрирует зависимости приложения, включая репозитории и сервисы.
    /// </summary>
    /// <param name="services">Коллекция сервисов для внедрения зависимостей.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddRepositories()
            .AddServices();
    }
    
    /// <summary>
    /// Регистрирует репозитории в контейнер зависимостей со временем жизни Scoped.
    /// </summary>
    /// <param name="services">Коллекция сервисов для внедрения зависимостей.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    private static IServiceCollection AddRepositories(this IServiceCollection services)
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
    
    /// <summary>
    /// Регистрирует бизнес-сервисы в контейнер зависимостей со временем жизни Scoped.
    /// </summary>
    /// <param name="services">Коллекция сервисов для внедрения зависимостей.</param>
    /// <returns>Обновленная коллекция сервисов.</returns>
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<ISystemInfoService, SystemInfoService>()
            .AddScoped<IAccountService, AccountService>()
            .AddScoped<IAccountTypeService, AccountTypeService>()
            .AddScoped<IBankService, BankService>()
            .AddScoped<ICategoryService, CategoryService>()
            .AddScoped<ICountryService, CountryService>()
            .AddScoped<ICurrencyService, CurrencyService>()
            .AddScoped<IExchangeRateService, ExchangeRateService>()
            .AddScoped<IRegistryHolderService, RegistryHolderService>();

        services.AddFluentResultsExceptionsFactories();
        
        return services;
    }

    /// <summary>
    /// Регистрирует фабрики ошибок для различных сущностей в контейнер зависимостей.
    /// Используется для централизованного создания экземпляров ошибок в стиле FluentResults.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации зависимостей.</param>
    /// <returns>Обновлённая коллекция сервисов.</returns>
    private static IServiceCollection AddFluentResultsExceptionsFactories(this IServiceCollection services)
    {
        services
            .AddScoped<IErrorsFactory, ErrorsFactory>()
            .AddScoped<IAccountErrorsFactory, AccountErrorsFactory>()
            .AddScoped<IAccountTypeErrorsFactory, AccountTypeErrorsFactory>()
            .AddScoped<IBankErrorsFactory, BankErrorsFactory>()
            .AddScoped<ICategoryErrorsFactory, CategoryErrorsFactory>()
            .AddScoped<ICountryErrorsFactory, CountryErrorsFactory>()
            .AddScoped<ICurrencyErrorsFactory, CurrencyErrorsFactory>()
            .AddScoped<IExchangeRateErrorsFactory, ExchangeRateErrorsFactory>()
            .AddScoped<IRegistryHolderErrorsFactory, RegistryHolderErrorsFactory>();
        return services;
    }
}