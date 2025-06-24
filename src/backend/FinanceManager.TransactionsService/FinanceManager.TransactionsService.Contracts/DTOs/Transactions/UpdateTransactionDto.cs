namespace FinanceManager.TransactionsService.Contracts.DTOs.Transactions;

/// <summary>
/// DTO для обновления транзакции
/// </summary>
/// <param name="Id">Идентификатор транзакции</param>
/// <param name="Date">Дата транзакции</param>
/// <param name="Account">Счет по которому проведена транзакция</param>
/// <param name="Category">Категория на которую относится транзакция</param>
/// <param name="Amount">Сумма транзакции</param>
/// <param name="Description">Описание транзакции</param>
public record UpdateTransactionDto(
    Guid Id,
    DateTime? Date = null,
    Guid? Account  = null,
    Guid? Category = null,
    decimal? Amount  = null,
    string? Description = null 
);