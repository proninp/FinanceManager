using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class Country(string name) : IdentityEntity
{
    public string Name { get; set; } = name;
}