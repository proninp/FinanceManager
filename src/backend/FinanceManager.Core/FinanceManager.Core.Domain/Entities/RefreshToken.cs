namespace FinanceManager.Core.Domain.Entities;

public class RefreshToken
{
    public int Id { get; init; }
    public Guid UserId { get; init; }
    public User User { get; init; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
}