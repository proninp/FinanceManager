namespace FinanceManager.CatalogService.Contracts.DTOs.ExchangeRates;

/// <summary>
/// DTO для обновления обменного курса валюты
/// </summary>
/// <param name="Id">Идентификатор курса</param>
/// <param name="RateDate">Дата курса</param>
/// <param name="CurrencyId">Идентификатор валюты</param>
/// <param name="Rate">Значение курса</param>
public record UpdateExchangeRateDto(
    Guid Id,
    DateTime? RateDate = null,
    Guid? CurrencyId = null,
    decimal? Rate = null
);