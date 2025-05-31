namespace FinanceManager.TelegramService.Domain.Entities;

/// <summary>
/// Представляет состояние в конечном автомате Telegram-бота
/// </summary>
/// <param name="name">Название состояния</param>
public class State(string name) : IdentityModel
{
    /// <summary>
    /// Уникальное название состояния в системе состояний бота
    /// </summary>
    public string Name { get; set; } = name;
}