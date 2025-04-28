namespace FinanceManager.Core.Domain.Entities;

public class Category
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public List<Category> SubCategories { get; set; }
    public bool Income { get; set; }
    public bool Expense { get; set; }
    public bool Deleted { get; set; }
    public string emoji { get; set; }
}