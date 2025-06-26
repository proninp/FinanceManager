using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Contracts.DTOs.Categories;

/// <summary>
/// DTO для создания категории
/// </summary>
/// <param name="RegistryHolderId">Идентификатор владельца категории</param>
/// <param name="Name">Название категории</param>
/// <param name="Income">Является ли категория доходной</param>
/// <param name="Expense">Является ли категория расходной</param>
/// <param name="Emoji">Эмодзи категории</param>
/// <param name="Icon">Иконка категории</param>
/// <param name="ParentId">Идентификатор родительской категории</param>
public record CreateCategoryDto(
    Guid RegistryHolderId,
    string Name,
    bool Income,
    bool Expense,
    string? Emoji = null,
    byte[]? Icon = null,
    Guid? ParentId = null
);

/// <summary>
/// Методы-расширения для преобразования CreateCategoryDto в Category
/// </summary>
public static class CreateCategoryDtoExtensions
{
    /// <summary>
    /// Преобразует DTO создания категории в сущность Category
    /// </summary>
    /// <param name="dto">DTO для создания категории</param>
    /// <returns>Экземпляр Category</returns>
    public static Category ToCategory(this CreateCategoryDto dto) =>
        new Category(
            dto.RegistryHolderId,
            dto.Name,
            dto.Income,
            dto.Expense,
            dto.Emoji,
            dto.Icon,
            dto.ParentId
        );
}