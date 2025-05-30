using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

public class Country(string name) : IdentityModel
{
    public string Name { get; set; } = name;
}