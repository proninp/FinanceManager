using FinanceManager.UserService.Domain.Abstractions;
using FinanceManager.UserService.Domain.Enums;

namespace FinanceManager.UserService.Domain.Entities;

/// <summary>
/// Представляет пользователя системы финансового менеджера.
/// </summary>
/// <remarks>
/// Класс <see cref="User"/> описывает сущность пользователя, включающую основные данные,
/// такие как имя, электронная почта, хэш пароля, роль, идентификатор Telegram и часовой пояс.
/// Используется в контексте Identity и содержит ссылку на часовой пояс по умолчанию.
/// </remarks>
/// <param name="role">Роль пользователя в системе (например, администратор, пользователь).</param>
/// <param name="name">Имя пользователя.</param>
/// <param name="email">Адрес электронной почты пользователя.</param>
/// <param name="passwordHash">Хэш пароля для безопасного хранения.</param>
/// <param name="defaultTimeZoneId">Идентификатор часового пояса по умолчанию для пользователя.</param>
/// <param name="telegramId">Уникальный идентификатор пользователя в Telegram.</param>
public class User(Role role, string name, string passwordHash, Guid defaultTimeZoneId, long telegramId, string? email = null)
    : IdentityModel
{
    /// <summary>
    /// Роль пользователя в системе финансового менеджера
    /// </summary>
    public Role Role { get; set; } = role;
    
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; } = name;
    
    /// <summary>
    /// Адрес электронной почты
    /// </summary>
    public string? Email { get; set; } = email;
    
    /// <summary>
    /// Хэшсумма пароля пользователя
    /// </summary>
    public string PasswordHash { get; set; } = passwordHash;
    
    /// <summary>
    /// Идентификатор пользователя в Telegram
    /// </summary>
    public long TelegramId { get; set; } = telegramId;
    
    /// <summary>
    /// Идентификатор часового пояса пользователя
    /// </summary>
    public Guid DefaultTimeZoneId { get; set; } = defaultTimeZoneId;
    
    /// <summary>
    /// Часовой пояс пользователя по-умолчанию
    /// </summary>
    public TimeZone DefaultTimeZone { get; set; } = null!;
}