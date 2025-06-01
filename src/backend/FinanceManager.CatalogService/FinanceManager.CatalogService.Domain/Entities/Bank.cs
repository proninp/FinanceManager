using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

/// <summary>
/// Представляет банковское учреждение
/// </summary>
/// <param name="countryId">Идентификатор страны расположения банка</param>
/// <param name="name">Название банка</param>
public class Bank(Guid countryId, string name) : IdentityModel
{
    /// <summary>
    /// Идентификатор страны, в которой находится банк
    /// </summary>
    public Guid CountryId { get; set; } = countryId;

    /// <summary>
    /// Страна расположения банка
    /// </summary>
    public Country Country { get; set; } = null!;
    
    /// <summary>
    /// Название банка
    /// </summary>
    public string Name { get; set; } = name;
}