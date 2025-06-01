using FinanceManager.CatalogService.Domain.Abstractions;

namespace FinanceManager.CatalogService.Domain.Entities;

/// <summary>
/// Представляет справочник обменного курса валют на определенную дату
/// </summary>
/// <param name="rateDate">Дата курса</param>
/// <param name="currencyId">Идентификатор валюты</param>
/// <param name="rate">Значение курса</param>
public class ExchangeRate(DateTime rateDate, Guid currencyId, decimal rate) : IdentityModel
{
    /// <summary>
    /// Дата, на которую действует курс валюты
    /// </summary>
    public DateTime RateDate { get; set; } = rateDate;

    /// <summary>
    /// Идентификатор валюты
    /// </summary>
    public Guid CurrencyId { get; set; } = currencyId;

    /// <summary>
    /// Валюта, для которой установлен курс
    /// </summary>
    public Currency Currency { get; set; } = null!;

    /// <summary>
    /// Значение обменного курса
    /// </summary>
    public decimal Rate { get; set; } = rate;
}