using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

/// <summary>
/// Представляет справочник стран
/// </summary>
/// <param name="name">Название страны</param>
public class Country(string name) : IdentityModel
{
    /// <summary>
    /// Название страны
    /// </summary>
    public string Name { get; set; } = name;
}