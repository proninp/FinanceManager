using FinanceManager.UserService.Domain.Abstractions;

namespace FinanceManager.UserService.Domain.Entities;

/// <summary>
/// Модель часового пояса.
/// </summary>
/// <param name="name">Название часового пояса.</param>
public class TimeZone(string name) : IdentityModel
{
    /// <summary>
    /// Название часового пояса
    /// </summary>
    public string Name { get; set; } = name;
}