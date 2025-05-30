namespace FinanceManager.CatalogService.Domain.Abstractions;

public abstract class IdentityModel
{
    public Guid Id { get; init; }
    
    public DateTime CreatedAt { get; init; }
}