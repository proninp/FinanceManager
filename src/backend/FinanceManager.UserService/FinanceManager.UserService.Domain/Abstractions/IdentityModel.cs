namespace FinanceManager.UserService.Domain.Abstractions;

/// <summary>
/// Базовый абстрактный класс для всех доменных сущностей с идентификацией
/// </summary>
public class IdentityModel
{
    /// <summary>
    /// Уникальный идентификатор сущности
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Дата и время создания сущности
    /// </summary>
    public DateTime CreatedAt { get; init; }
}