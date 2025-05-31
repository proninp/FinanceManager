namespace FinanceManager.TelegramService.Domain.Entities;

/// <summary>
/// Представляет текущее состояние пользователя в конечном автомате Telegram-бота
/// </summary>
/// <param name="telegramUserId">Идентификатор пользователя Telegram</param>
/// <param name="stateId">Идентификатор состояния</param>
/// <param name="context">Контекст состояния пользователя</param>
public class UserState(Guid telegramUserId, Guid stateId, string context) : IdentityModel
{
    /// <summary>
    /// Идентификатор пользователя Telegram-бота
    /// </summary>
    public Guid TelegramUserId { get; set; } = telegramUserId;

    /// <summary>
    /// Идентификатор текущего состояния пользователя
    /// </summary>
    public Guid StateId { get; set; } = stateId;

    /// <summary>
    /// Контекстная информация состояния в формате JSON или строки
    /// </summary>
    public string Context { get; set; } = context;
}