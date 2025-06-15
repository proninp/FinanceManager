using FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;

namespace FinanceManager.TransactionsService.Contracts.DTOs.Transactions;

public record TransactionDto(
    Guid Id,
    TransactionAccountDto Account,
    TransactionCategoryDto Category,
    decimal Amount,
    string? Description
    );