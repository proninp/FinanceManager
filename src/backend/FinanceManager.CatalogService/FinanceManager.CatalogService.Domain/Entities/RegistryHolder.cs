using FinanceManager.CatalogService.Domain.Abstractions;
using FinanceManager.CatalogService.Domain.Enums;

namespace FinanceManager.CatalogService.Domain.Entities;

/// <summary>
/// Представляет владельца реестра финансовых данных
/// </summary>
/// <param name="telegramId">Идентификатор пользователя в Telegram</param>
/// <param name="role">Роль пользователя в системе</param>
public class RegistryHolder(long telegramId, Role role) : IdentityModel
{
    /// <summary>
    /// Уникальный идентификатор пользователя в Telegram
    /// </summary>
    public long TelegramId { get; set; } = telegramId;

    /// <summary>
    /// Роль пользователя в системе финансового менеджера
    /// </summary>
    public Role Role { get; set; } = role;
}