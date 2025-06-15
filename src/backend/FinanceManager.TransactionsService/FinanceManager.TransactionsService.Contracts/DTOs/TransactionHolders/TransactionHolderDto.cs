using FinanceManager.TransactionsService.Domain.Enums;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;

public record TransactionHolderDto(
    Guid Id,
    Role Role,
    long? TelegramId
    );