namespace FinanceManager.Core.Domain.Entities;

public class Bank
{
    public Guid Id { get; init; }
    public string Name { get; set; }
    public Guid CountryId { get; set; }
    public Country Country { get; set; }
}