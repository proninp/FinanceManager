namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;

/// <summary>
/// DTO для обновления банковского счета пользователя
/// </summary>
/// <param name="Id">Идентификатор счета</param>
/// <param name="AccountTypeId">Идентификатор типа счета</param>
/// <param name="CurrencyId">Идентификатор валюты счета</param>
/// <param name="CreditLimit">Кредитный лимит счета</param>
public record UpdateTransactionAccountDto(
    Guid Id,
    Guid? AccountTypeId = null,
    Guid? CurrencyId = null,
    decimal? CreditLimit = null
);