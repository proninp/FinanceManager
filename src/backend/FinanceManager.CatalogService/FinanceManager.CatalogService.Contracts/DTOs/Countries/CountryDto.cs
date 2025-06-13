using FinanceManager.CatalogService.Domain.Entities;

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

/// <summary>
/// Методы-расширения для преобразования сущности Country в CountryDto
/// </summary>
public static class CountryDtoExtensions
{
    /// <summary>
    /// Преобразует сущность Country в DTO CountryDto
    /// </summary>
    /// <param name="country">Сущность страны</param>
    /// <returns>Экземпляр CountryDto</returns>
    public static CountryDto ToDto(this Country country) =>
        new CountryDto(country.Id, country.Name);

    /// <summary>
    /// Преобразует коллекцию Country в коллекцию CountryDto
    /// </summary>
    /// <param name="countries">Коллекция сущностей стран</param>
    /// <returns>Коллекция CountryDto</returns>
    public static IEnumerable<CountryDto> ToDto(this IEnumerable<Country> countries) =>
        countries.Select(country => country.ToDto());
}