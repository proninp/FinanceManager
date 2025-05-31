using FinanceManager.TelegramService.Domain.Enums;

namespace FinanceManager.TelegramService.Domain.Entities;

/// <summary>
/// Представляет пользователя Telegram-бота с привязкой к основной системе
/// </summary>
/// <param name="telegramId">Идентификатор пользователя в Telegram</param>
/// <param name="userId">Идентификатор пользователя в основной системе</param>
/// <param name="role">Роль пользователя</param>
/// <param name="lastActivity">Время последней активности пользователя</param>
public class User(long telegramId, Guid userId, Role role, DateTime lastActivity) : IdentityModel
{
    /// <summary>
    /// Уникальный идентификатор пользователя в Telegram
    /// </summary>+
    public long TelegramId { get; set; } = telegramId;

    /// <summary>
    /// Идентификатор пользователя в основной системе FinanceManager
    /// </summary>
    public Guid UserId { get; set; } = userId;

    /// <summary>
    /// Роль пользователя в системе
    /// </summary>
    public Role Role { get; set; } = role;

    /// <summary>
    /// Дата и время последней активности пользователя в боте
    /// </summary>
    public DateTime LastActivity { get; set; } = lastActivity;
}