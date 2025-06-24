using FinanceManager.TransactionsService.Domain.Enums;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;

/// <summary>
/// DTO для создания владельца реестра
/// </summary>
/// <param name="TelegramId">Идентификатор пользователя в Telegram</param>
/// <param name="Role">Роль пользователя в системе</param>
public record CreateTransactionHolderDto(
    long TelegramId,
    Role Role = Role.User
);