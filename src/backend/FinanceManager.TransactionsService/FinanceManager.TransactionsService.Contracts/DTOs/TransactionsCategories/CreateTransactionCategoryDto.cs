namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;

/// <summary>
/// DTO для создания категории
/// </summary>
/// <param name="TransactionHolderId">Идентификатор владельца категории</param>
/// <param name="Income">Является ли категория доходной</param>
/// <param name="Expense">Является ли категория расходной</param>
public record CreateTransactionCategoryDto(
    Guid TransactionHolderId,
    bool Income,
    bool Expense
);