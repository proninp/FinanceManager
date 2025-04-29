using FinanceManager.Core.Domain.Abstractions;

namespace FinanceManager.Core.Domain.Entities;

public class ExchangeRate(DateTime rateDate, Guid currencyId, decimal rate) : IdentityEntity
{
    public DateTime RateDate { get; set; } = rateDate;

    public Guid CurrencyId { get; set; } = currencyId;
    
    public Currency Currency { get; set; } = null!;

    public decimal Rate { get; set; } = rate;
}