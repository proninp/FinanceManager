using FinanceManager.Core.Domain.Abstractions;
using FinanceManager.Core.Domain.Enums;

namespace FinanceManager.Core.Domain.Entities;

public class User(string name, string email, string passwordHash, Guid defaultTimeZoneId, long telegramId)
    : IdentityEntity
{
    public string Name { get; set; } = name;
    
    public string Email { get; set; } = email;
    
    public string PasswordHash { get; set; } = passwordHash;
    
    public Guid DefaultTimeZoneId { get; set; } = defaultTimeZoneId;
    
    public TimeZone DefaultTimeZone { get; set; } = null!;
    
    public long TelegramId { get; set; } = telegramId;
    
    public UserRole Role { get; set; } = UserRole.User;
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    
    public ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
}