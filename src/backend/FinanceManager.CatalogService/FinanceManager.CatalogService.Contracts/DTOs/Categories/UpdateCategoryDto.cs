namespace FinanceManager.CatalogService.Contracts.DTOs.Categories;

/// <summary>
/// DTO для обновления категории
/// </summary>
/// <param name="Id">Идентификатор категории</param>
/// <param name="RegistryHolderId">Идентификатор владельца категории</param>
/// <param name="Name">Название категории</param>
/// <param name="Income">Является ли категория доходной</param>
/// <param name="Expense">Является ли категория расходной</param>
/// <param name="Emoji">Эмодзи категории</param>
/// <param name="Icon">Иконка категории</param>
/// <param name="ParentId">Идентификатор родительской категории</param>
public record UpdateCategoryDto(
    Guid Id,
    Guid? RegistryHolderId,
    string? Name,
    bool? Income,
    bool? Expense,
    string? Emoji,
    byte[]? Icon,
    Guid? ParentId
);