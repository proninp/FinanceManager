using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

public class ExchangeRate(DateTime rateDate, Guid currencyId, decimal rate) : IdentityModel
{
    public DateTime RateDate { get; set; } = rateDate;

    public Guid CurrencyId { get; set; } = currencyId;

    public Currency Currency { get; set; } = null!;

    public decimal Rate { get; set; } = rate;
}