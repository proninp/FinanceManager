using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

public class Category(
    Guid registryHolderId,
    string name,
    bool income,
    bool expense,
    string? emoji,
    byte[]? icon,
    Guid? parentId) : IdentityModel
{
    public Guid RegistryHolderId { get; set; } = registryHolderId;
    
    public RegistryHolder RegistryHolder { get; set; } = null!;

    public string Name { get; set; } = name;

    public bool Income { get; set; } = income;

    public bool Expense { get; set; } = expense;

    public string? Emoji { get; set; } = emoji;

    public byte[]? Icon { get; set; } = icon;

    public Guid? ParentId { get; set; } = parentId;
    
    public Category? Parent { get; set; }
}