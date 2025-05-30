using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

public class Currency(string name, string charCode, string numCode, string? sign, string? emoji, bool isDeleted)
    : IdentityModel
{
    public string Name { get; set; } = name;

    public string CharCode { get; set; } = charCode;

    public string NumCode { get; set; } = numCode;

    public string? Sign { get; set; } = sign;

    public string? Emoji { get; set; } = emoji;

    public bool IsDeleted { get; set; } = isDeleted;
}