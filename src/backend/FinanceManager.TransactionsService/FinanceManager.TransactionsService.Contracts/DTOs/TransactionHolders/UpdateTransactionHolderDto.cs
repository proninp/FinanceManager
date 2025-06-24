using FinanceManager.TransactionsService.Domain.Enums;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;

/// <summary>
/// DTO для обновления владельца транзакций
/// </summary>
/// <param name="Id">Уникальный идентификатор владельца</param>
/// <param name="TelegramId">Идентификатор пользователя в Telegram</param>
/// <param name="Role">Роль пользователя в системе</param>
public record UpdateTransactionHolderDto(
    Guid Id,
    long? TelegramId = null,
    Role? Role = null
);