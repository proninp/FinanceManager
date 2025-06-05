namespace FinanceManager.CatalogService.Contracts.DTOs.Countries;

/// <summary>
/// DTO для создания страны
/// </summary>
/// <param name="Name">Название страны</param>
public record CreateCountryDto(
    string Name
);