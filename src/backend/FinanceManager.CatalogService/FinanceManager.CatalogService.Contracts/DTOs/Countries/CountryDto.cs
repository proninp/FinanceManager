namespace FinanceManager.CatalogService.Contracts.DTOs.Countries;

/// <summary>
/// DTO для страны
/// </summary>
/// <param name="Id">Идентификатор страны</param>
/// <param name="Name">Название страны</param>
public record CountryDto(
    Guid Id,
    string Name
);