using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class RefreshToken(Guid userId, string token, DateTime expiresAt, bool isRevoked = false) : IdentityEntity
{
    public Guid UserId { get; init; } = userId;
    public User User { get; init; } = null!;
    public string Token { get; set; } = token;
    public DateTime ExpiresAt { get; set; } = expiresAt;
    public bool IsRevoked { get; set; } = isRevoked;
}