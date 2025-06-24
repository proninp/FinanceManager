using FinanceManager.TransactionsService.Contracts.DTOs.AccountTypes;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionCurrencies;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;

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