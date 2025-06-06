using FinanceManager.UserService.Domain.Abstractions;

namespace FinanceManager.UserService.Domain.Entities;

/// <summary>
/// Представляет модель токена обновления, используемого для продления срока действия access токена.
/// </summary>
/// <param name="userId">Идентификатор пользователя, которому принадлежит токен.</param>
/// <param name="token">Строковое значение токена.</param>
/// <param name="expiresAt">Дата и время истечения срока действия токена.</param>
/// <param name="isRevoked">Указывает, был ли токен отозван.</param>
public class RefreshToken(Guid userId, string token, DateTime expiresAt, bool isRevoked = false) : IdentityModel
{
    /// <summary>
    /// Идентификатор пользователя - владельца токена
    /// </summary>
    public Guid UserId { get; set; } = userId;
    
    /// <summary>
    /// Пользователь - владелец токена
    /// </summary>
    public User User { get; set; } = null!;
    
    /// <summary>
    /// Токен
    /// </summary>
    public string Token { get; set; } = token;
    
    /// <summary>
    /// Дата истечения срока годности токена
    /// </summary>
    public DateTime ExpiresAt { get; set; } = expiresAt;
    
    /// <summary>
    /// Флаг отозванности токенв
    /// </summary>
    public bool IsRevoked { get; set; } = isRevoked;
}