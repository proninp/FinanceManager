using FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;

public record TransactionCategoryDto(
    Guid Id,
    TransactionHolderDto Holder,
    bool Income,
    bool Expense
    );