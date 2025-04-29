using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class Category(
    string name,
    Guid userId,
    Guid? parentCategoryId,
    bool income,
    bool expense,
    bool deleted = false,
    string emoji = null!) : IdentityEntity
{
    public string Name { get; set; } = name;
    public Guid UserId { get; set; } = userId;
    public User User { get; set; } = null!;
    public Guid? ParentCategoryId { get; set; } = parentCategoryId;
    public Category? ParentCategory { get; set; } = null!;
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public bool Income { get; set; } = income;
    public bool Expense { get; set; } = expense;
    public bool Deleted { get; set; } = deleted;
    public string? Emoji { get; set; } = emoji;
}