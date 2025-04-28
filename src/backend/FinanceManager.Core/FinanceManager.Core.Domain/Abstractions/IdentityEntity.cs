namespace FinanceManager.Core.Domain.Abstractions;

public abstract class IdentityEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.Now;
}