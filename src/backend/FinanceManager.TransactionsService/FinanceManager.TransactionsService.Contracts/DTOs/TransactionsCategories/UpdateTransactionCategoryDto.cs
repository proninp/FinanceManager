namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;

/// <summary>
/// DTO для обновления категории
/// </summary>
/// <param name="Id">Идентификатор категории</param>
/// <param name="Income">Является ли категория доходной</param>
/// <param name="Expense">Является ли категория расходной</param>

public record UpdateTransactionCategoryDto(
    Guid Id,
    bool? Income = null,
    bool? Expense = null
);