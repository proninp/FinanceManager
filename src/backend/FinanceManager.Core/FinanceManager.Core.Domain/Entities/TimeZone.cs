using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class TimeZone(string name) :IdentityEntity
{
    public string Name { get; set; } = name;
}