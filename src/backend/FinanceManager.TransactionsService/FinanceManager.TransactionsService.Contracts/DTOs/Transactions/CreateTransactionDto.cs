using FinanceManager.TransactionsService.Domain.Entities;

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

public static class CreateTransactionDtoExtensions
{
    /// <summary>
    /// Преобразует CreateTransactionDto в Transaction
    /// </summary>
    public static Transaction ToTransaction(this CreateTransactionDto dto)
    {
        return new Transaction(
            date: dto.Date,
            accountId: dto.AccountId,
            categoryId: dto.CategoryId,
            amount: dto.Amount,
            description: dto.Description
        );
    }
}