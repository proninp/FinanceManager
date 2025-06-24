using FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;

namespace FinanceManager.TransactionsService.Contracts.DTOs.Transactions;

/// <summary>
/// DTO для транзакции
/// </summary>
/// <param name="Id">Идентификатор транзакции</param>
/// <param name="Date">Дата транзакции</param>
/// <param name="Account">Счет по которому проведена транзакция</param>
/// <param name="Category">Категория на которую относится транзакция</param>
/// <param name="Amount">Сумма транзакции</param>
/// <param name="Description">Описание транзакции</param>
public record TransactionDto(
    Guid Id,
    DateTime Date,
    TransactionAccountDto Account,
    TransactionCategoryDto Category,
    decimal Amount,
    string? Description
    );