using FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;
using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Contracts.DTOs.Categories;

/// <summary>
/// DTO для категории
/// </summary>
/// <param name="Id">Идентификатор категории</param>
/// <param name="RegistryHolder">Владеец категории</param>
/// <param name="Name">Название категории</param>
/// <param name="Income">Является ли категория доходной</param>
/// <param name="Expense">Является ли категория расходной</param>
/// <param name="Emoji">Эмодзи категории</param>
/// <param name="Icon">Иконка категории</param>
/// <param name="ParentId">Идентификатор родительской категории</param>
/// <param name="ParentName">Наименование родительской категории</param>
public record CategoryDto(
    Guid Id,
    RegistryHolderDto RegistryHolder,
    string Name,
    bool Income,
    bool Expense,
    string? Emoji = null,
    byte[]? Icon = null,
    Guid? ParentId = null,
    string? ParentName = null
);

/// <summary>
/// Методы-расширения для преобразования сущности Category в CategoryDto
/// </summary>
public static class CategoryDtoExtensions
{
    /// <summary>
    /// Преобразует сущность Category в DTO CategoryDto
    /// </summary>
    /// <param name="category">Сущность категории</param>
    /// <returns>Экземпляр CategoryDto</returns>
    public static CategoryDto ToDto(this Category category) =>
        new CategoryDto(
            category.Id,
            category.RegistryHolder.ToDto(),
            category.Name,
            category.Income,
            category.Expense,
            category.Emoji,
            category.Icon,
            category.ParentId,
            category.Parent?.Name
        );

    /// <summary>
    /// Преобразует коллекцию Category в коллекцию CategoryDto
    /// </summary>
    /// <param name="categories">Коллекция сущностей категорий</param>
    /// <returns>Коллекция CategoryDto</returns>
    public static IEnumerable<CategoryDto> ToDto(this IEnumerable<Category> categories) =>
        categories.Select(ToDto);
}