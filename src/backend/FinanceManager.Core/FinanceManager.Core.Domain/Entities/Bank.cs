using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class Bank(string name, Guid countryId) : IdentityEntity
{
    public string Name { get; set; } = name;
    
    public Guid CountryId { get; set; } = countryId;
    
    public Country Country { get; set; } = null!;
}