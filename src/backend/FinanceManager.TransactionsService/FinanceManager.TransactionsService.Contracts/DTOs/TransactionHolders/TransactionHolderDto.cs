using FinanceManager.TransactionsService.Domain.Enums;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;

/// <summary>
/// DTO для владельца транзакций
/// </summary>
/// <param name="Id">Идентификатор владельца транзакций</param>
/// <param name="TelegramId">Идентификатор пользователя в Telegram</param>
/// <param name="Role">Роль пользователя в системе</param>
public record TransactionHolderDto(
    Guid Id,
    Role Role,
    long? TelegramId
    );