using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

public class AccountType(string code, string description, bool isDeleted = false) : IdentityModel
{
    public string Code { get; set; } =  code;

    public string Description { get; set; } = description;

    public bool IsDeleted { get; set; } = isDeleted;
}