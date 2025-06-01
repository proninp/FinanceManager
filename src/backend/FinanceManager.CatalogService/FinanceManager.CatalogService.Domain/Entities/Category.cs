using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

/// <summary>
/// Представляет категорию доходов или расходов
/// </summary>
/// <param name="registryHolderId">Идентификатор владельца категории</param>
/// <param name="name">Название категории</param>
/// <param name="income">Является ли категория доходной</param>
/// <param name="expense">Является ли категория расходной</param>
/// <param name="emoji">Эмодзи для категории</param>
/// <param name="icon">Иконка категории в виде массива байтов</param>
/// <param name="parentId">Идентификатор родительской категории</param>
public class Category(
    Guid registryHolderId,
    string name,
    bool income,
    bool expense,
    string? emoji,
    byte[]? icon,
    Guid? parentId) : IdentityModel
{
    /// <summary>
    /// Идентификатор владельца категории
    /// </summary>
    public Guid RegistryHolderId { get; set; } = registryHolderId;
    
    /// <summary>
    /// Владелец категории
    /// </summary>
    public RegistryHolder RegistryHolder { get; set; } = null!;

    /// <summary>
    /// Название категории
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Флаг, указывающий, что категория предназначена для доходов
    /// </summary>
    public bool Income { get; set; } = income;

    /// <summary>
    /// Флаг, указывающий, что категория предназначена для расходов
    /// </summary>
    public bool Expense { get; set; } = expense;

    /// <summary>
    /// Эмодзи для визуального представления категории
    /// </summary>
    public string? Emoji { get; set; } = emoji;

    /// <summary>
    /// Иконка категории в виде массива байтов
    /// </summary>
    public byte[]? Icon { get; set; } = icon;

    /// <summary>
    /// Идентификатор родительской категории для создания иерархии
    /// </summary>
    public Guid? ParentId { get; set; } = parentId;
    
    /// <summary>
    /// Родительская категория
    /// </summary>
    public Category? Parent { get; set; }
}