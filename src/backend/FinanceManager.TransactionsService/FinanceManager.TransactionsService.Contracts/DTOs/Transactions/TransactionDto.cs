using FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;
using FinanceManager.TransactionsService.Domain.Entities;

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

/// <summary>
/// Методы-расширения для преобразования сущности Transaction в TransactionDto
/// </summary>
public static class TransactionDtoExtensions
{
    /// <summary>
    /// Преобразует сущность Transaction в DTO TransactionDto
    /// </summary>
    /// <param name="transaction">Сущность транзакции — записи о доходе или расходе средств</param>
    /// <returns>Экземпляр TransactionDto</returns>
    public static TransactionDto ToDto(this Transaction transaction)
    {
        return new TransactionDto(
            transaction.Id,
            transaction.Date,
            transaction.Account.ToDto(),
            transaction.Category.ToDto(),
            transaction.Amount,
            transaction.Description
        );
    }

    /// <summary>
    /// Преобразует коллекцию сущностей Transaction в коллекцию DTO TransactionDto
    /// </summary>
    /// <param name="transactions">Коллекция сущностей транзакций</param>
    /// <returns>Коллекция TransactionDto</returns>
    public static ICollection<TransactionDto> ToDto(this IEnumerable<Transaction> transactions) =>
        transactions.Select(ToDto).ToList();
}