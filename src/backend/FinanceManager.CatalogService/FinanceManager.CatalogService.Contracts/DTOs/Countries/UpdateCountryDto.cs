namespace FinanceManager.CatalogService.Contracts.DTOs.Countries;

/// <summary>
/// DTO для обновления страны
/// </summary>
/// <param name="Id">Идентификатор страны</param>
/// <param name="Name">Название страны</param>
public record UpdateCountryDto(
    Guid Id,
    string? Name
);