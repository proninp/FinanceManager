using FinanceManager.TransactionsService.Domain.Abstractions;
using FinanceManager.TransactionsService.Domain.Enums;

namespace FinanceManager.TransactionsService.Domain.Entities;

/// <summary>
/// Представляет участника системы, владельца транзакции
/// </summary>
/// <param name="role">Роль пользователя в системе (пользователь или администратор)</param>
/// <param name="telegramId">Необязательный идентификатор пользователя в Telegram</param>
public class TransactionHolder(Role role, long? telegramId = null):IdentityModel
{
    /// <summary>
    /// Роль пользователя в системе финансового менеджера
    /// </summary>
    public Role Role { get; set; } = role;

    /// <summary>
    /// Уникальный идентификатор пользователя в Telegram (если доступ осуществляется через Telegram-бота)
    /// </summary>
    public long? TelegramId { get; set; } = telegramId;
}