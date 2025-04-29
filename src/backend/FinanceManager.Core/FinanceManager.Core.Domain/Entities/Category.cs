using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class Category(
    string name,
    Guid userId,
    Guid? parentCategoryId,
    bool income,
    bool expense,
    bool isDeleted = false,
    string? emoji = null,
    byte[]? icon = null) : IdentityEntity
{
    public string Name { get; set; } = name;
    
    public Guid UserId { get; set; } = userId;
    
    public User User { get; set; } = null!;
    
    public Guid? ParentCategoryId { get; set; } = parentCategoryId;
    
    public Category? ParentCategory { get; set; } = null!;
    
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    
    public bool Income { get; set; } = income;
    
    public bool Expense { get; set; } = expense;
    
    public bool IsDeleted { get; set; } = isDeleted;
    
    public string? Emoji { get; set; } = emoji;

    public byte[]? Icon { get; set; } = icon;
    
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}