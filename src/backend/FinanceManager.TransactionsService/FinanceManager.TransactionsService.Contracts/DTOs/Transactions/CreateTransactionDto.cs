namespace FinanceManager.TransactionsService.Contracts.DTOs.Transactions;

/// <summary>
/// DTO для создания транзакции
/// </summary>
/// <param name="Date">Дата транзакции</param>
/// <param name="AccountId">Идентификатор счета транзакции</param>
/// <param name="CategoryId">Идентификатор категории транзакции</param>
/// <param name="Amount">Сумма транзакции</param>
/// <param name="Description">Описание транзакции</param>
public record CreateTransactionDto(
    DateTime Date,
    Guid AccountId,
    Guid CategoryId,
    decimal Amount,
    string? Description
);