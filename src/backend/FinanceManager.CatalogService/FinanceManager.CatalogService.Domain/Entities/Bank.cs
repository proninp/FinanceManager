using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

public class Bank(Guid countryId, string name) : IdentityModel
{
    public Guid CountryId { get; set; } = countryId;

    public Country Country { get; set; } = null!;
    
    public string Name { get; set; } = name;
}