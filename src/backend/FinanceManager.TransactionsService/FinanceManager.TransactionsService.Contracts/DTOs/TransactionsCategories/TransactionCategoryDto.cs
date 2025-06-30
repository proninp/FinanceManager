using FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;
using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;

/// <summary>
/// DTO для представления категории транзакции
/// </summary>
/// <param name="Id">Уникальный идентификатор категории</param>
/// <param name="HolderId">Идентификатор владельца категории (пользователь или система)</param>
/// <param name="Income">Признак, указывающий, является ли категория доходной</param>
/// <param name="Expense">Признак, указывающий, является ли категория расходной</param>
public record TransactionCategoryDto(
    Guid Id,
    Guid HolderId,
    bool Income,
    bool Expense
);

/// <summary>
/// Методы-расширения для преобразования сущности TransactionsCategory в TransactionCategoryDto
/// </summary>
public static class TransactionCategoryDtoExtensions
{
    /// <summary>
    /// Преобразует сущность TransactionsCategory в DTO TransactionCategoryDto
    /// </summary>
    /// <param name="category">Сущность категории транзакции, принадлежащая конкретному пользователю</param>
    /// <returns>Экземпляр TransactionCategoryDto</returns>
    public static TransactionCategoryDto ToDto(this TransactionsCategory category)
    {
        return new TransactionCategoryDto(
            category.Id,
            category.HolderId,
            category.Income,
            category.Expense
        );
    }

    /// <summary>
    /// Преобразует коллекцию сущностей TransactionsCategory в коллекцию DTO TransactionCategoryDto
    /// </summary>
    /// <param name="categories">Коллекция сущностей категорий транзакций</param>
    /// <returns>Коллекция TransactionCategoryDto</returns>
    public static ICollection<TransactionCategoryDto> ToDto(this IEnumerable<TransactionsCategory> categories) =>
        categories.Select(ToDto).ToList();
}