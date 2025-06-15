using FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;

namespace FinanceManager.TransactionsService.Contracts.DTOs.Transfers;

public record TransferDto(
    Guid Id,
    TransactionAccountDto FromAccount,
    TransactionAccountDto ToAccount,
    decimal FromAmount,
    decimal ToAmount,
    string Description
    );