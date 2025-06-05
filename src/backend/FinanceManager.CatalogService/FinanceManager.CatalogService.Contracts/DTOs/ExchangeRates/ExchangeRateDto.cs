using FinanceManager.CatalogService.Contracts.DTOs.Currencies;

namespace FinanceManager.CatalogService.Contracts.DTOs.ExchangeRates;

/// <summary>
/// DTO для обменного курса валюты
/// </summary>
/// <param name="Id">Идентификатор курса</param>
/// <param name="RateDate">Дата курса</param>
/// <param name="Currency">Валюта</param>
/// <param name="Rate">Значение курса</param>
public record ExchangeRateDto(
    Guid Id,
    DateTime RateDate,
    CurrencyDto Currency,
    decimal Rate
);