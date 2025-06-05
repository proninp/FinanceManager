namespace FinanceManager.CatalogService.Contracts.DTOs.ExchangeRates;

/// <summary>
/// DTO для создания обменного курса валюты
/// </summary>
/// <param name="RateDate">Дата курса</param>
/// <param name="CurrencyId">Идентификатор валюты</param>
/// <param name="Rate">Значение курса</param>
public record CreateExchangeRateDto(
    DateTime RateDate,
    Guid CurrencyId,
    decimal Rate
);