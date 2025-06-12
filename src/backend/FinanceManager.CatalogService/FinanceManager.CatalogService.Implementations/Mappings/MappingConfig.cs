using Mapster;

namespace FinanceManager.CatalogService.Implementations.Mappings;

/// <summary>
/// Главный конфигуратор маппингов для всех сущностей
/// </summary>s
public static class MappingConfig
{
    /// <summary>
    /// Настраивает все маппинги проекта
    /// </summary>
    public static void Configure()
    {
        ConfigureGlobalSettings();
        
    }

    /// <summary>
    /// Настройка глобальных параметров Mapster
    /// </summary>
    private static void ConfigureGlobalSettings()
    {
        TypeAdapterConfig.GlobalSettings.Default
            .IgnoreNullValues(true) // Игнорирование null значений при маппинге
            .PreserveReference(true) // Сохранение ссылок для предотвращения циклических зависимостей
            .RequireDestinationMemberSource(false) // Не требует обязательного источника для каждого поля назначения
            .MaxDepth(5) // Максимальная глубина маппинга (защита от глубоких циклов)
            .EnumMappingStrategy(EnumMappingStrategy.ByName); // Настройка для enum
    }
}