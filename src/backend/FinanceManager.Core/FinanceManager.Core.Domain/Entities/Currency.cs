using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class Currency(
    string name,
    string charCode,
    string numCode,
    string? sign = null,
    string? emoji = null,
    bool isDeleted = false) : IdentityEntity
{
    public string Name { get; set; } = name;

    public string CharCode { get; set; } = charCode;

    public string NumCode { get; set; } = numCode;

    public string? Sign { get; set; } = sign;

    public string? Emoji { get; set; } = emoji;

    public bool IsDeleted { get; set; } = isDeleted;
    
    public ICollection<ExchangeRate> ExchangeRates { get; set; } = new List<ExchangeRate>();
}