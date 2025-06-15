using FinanceManager.TransactionsService.Contracts.DTOs.AccountTypes;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionCurrencies;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;

public record TransactionAccountDto(
    Guid Id,
    AccountTypeDto AccountType,
    TransactionCurrencyDto Currency,
    TransactionHolderDto Holder,
    decimal CreditLimit
    );