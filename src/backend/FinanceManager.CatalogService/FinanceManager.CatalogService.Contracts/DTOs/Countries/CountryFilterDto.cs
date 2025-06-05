namespace FinanceManager.CatalogService.Contracts.DTOs.Countries;

/// <summary>
/// DTO для фильтрации и пагинации стран
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="Name">Название страны</param>
public record CountryFilterDto(
    int ItemsPerPage,
    int Page,
    string? Name
);