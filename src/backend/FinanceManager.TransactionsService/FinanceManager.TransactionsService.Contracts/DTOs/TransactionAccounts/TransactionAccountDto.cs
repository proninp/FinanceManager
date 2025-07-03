using FinanceManager.TransactionsService.Contracts.DTOs.AccountTypes;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionCurrencies;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;
using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;

/// <summary>
/// DTO для банковского счета пользователя
/// </summary>
/// <param name="Id">Идентификатор счета</param>
/// <param name="Holder">Владелец счета</param>
/// <param name="AccountType">Тип счета</param>
/// <param name="Currency">Валюта счета</param>
/// <param name="CreditLimit">Кредитный лимит счета</param>
public record TransactionAccountDto(
    Guid Id,
    AccountTypeDto AccountType,
    TransactionCurrencyDto Currency,
    TransactionHolderDto Holder,
    decimal CreditLimit
);

/// <summary>
/// Методы-расширения для преобразования сущности TransactionsAccount в TransactionAccountDto
/// </summary>
public static class TransactionAccountDtoExtensions
{
    /// <summary>
    /// Преобразует сущность TransactionsAccount в DTO TransactionAccountDto
    /// </summary>
    /// <param name="account">Сущность банковского или финансового счета</param>
    /// <returns>Экземпляр TransactionAccountDto</returns>
    public static TransactionAccountDto ToDto(this TransactionsAccount account)
    {
        return new TransactionAccountDto(
            account.Id,
            account.AccountType.ToDto(),
            account.Currency.ToDto(),
            account.Holder.ToDto(),
            account.CreditLimit ?? 0m
        );
    }

    /// <summary>
    /// Преобразует коллекцию сущностей TransactionsAccount в коллекцию DTO TransactionAccountDto
    /// </summary>
    /// <param name="accounts">Коллекция сущностей банковских или финансовых счетов</param>
    /// <returns>Коллекция TransactionAccountDto</returns>
    public static ICollection<TransactionAccountDto> ToDto(this IEnumerable<TransactionsAccount> accounts)
    {
        return accounts.Select(ToDto).ToList();
    }
}