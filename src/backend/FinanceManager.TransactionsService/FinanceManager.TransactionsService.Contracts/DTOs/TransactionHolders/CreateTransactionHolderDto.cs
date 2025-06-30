using FinanceManager.TransactionsService.Domain.Entities;
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

public static class CreateTransactionHolderDtoExtensions
{
    /// <summary>
    /// Преобразует CreateTransactionHolderDto в TransactionHolder
    /// </summary>
    public static TransactionHolder ToHolder(this CreateTransactionHolderDto dto)
    {
        return new TransactionHolder(
            role: dto.Role,
            telegramId: dto.TelegramId
        );
    }
}