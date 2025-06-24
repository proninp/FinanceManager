using FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;

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