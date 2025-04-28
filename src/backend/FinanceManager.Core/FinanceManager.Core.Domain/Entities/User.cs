namespace FinanceManager.Core.Domain.Entities;

public class User
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public Guid DefaultTimeZoneId { get; set; }
    public TimeZone DefaultTimeZone { get; set; }
    public DateTime RegisteredAt { get; set; }
    public long TelegramId { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
}

public enum UserRole
{
    Admin,
    User
}