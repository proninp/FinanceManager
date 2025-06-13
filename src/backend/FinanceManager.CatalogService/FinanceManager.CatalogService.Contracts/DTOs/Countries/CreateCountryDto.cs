using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Contracts.DTOs.Countries;

/// <summary>
/// DTO для создания страны
/// </summary>
/// <param name="Name">Название страны</param>
public record CreateCountryDto(
    string Name
);

/// <summary>
/// Методы-расширения для преобразования CreateCountryDto в Country
/// </summary>
public static class CreateCountryDtoExtensions
{
    /// <summary>
    /// Преобразует DTO создания страны в сущность Country
    /// </summary>
    /// <param name="dto">DTO для создания страны</param>
    /// <returns>Экземпляр Country</returns>
    public static Country ToCountry(this CreateCountryDto dto) =>
        new Country(dto.Name);
}