using FinanceManager.CatalogService.Contracts.DTOs.Currencies;
using FinanceManager.CatalogService.Domain.Entities;

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

/// <summary>
/// Методы-расширения для преобразования ExchangeRate в ExchangeRateDto
/// </summary>
public static class ExchangeRateDtoExtensions
{
    /// <summary>
    /// Преобразует сущность ExchangeRate в DTO ExchangeRateDto
    /// </summary>
    /// <param name="exchangeRate">Сущность обменного курса</param>
    /// <returns>Экземпляр ExchangeRateDto</returns>
    public static ExchangeRateDto ToDto(this ExchangeRate exchangeRate) =>
        new ExchangeRateDto(
            exchangeRate.Id,
            exchangeRate.RateDate,
            exchangeRate.Currency.ToDto(),
            exchangeRate.Rate
            );
    
    /// <summary>
    /// Преобразует коллекцию сущностей ExchangeRate в коллекцию DTO ExchangeRateDto
    /// </summary>
    /// <param name="exchangeRates">Коллекция сущностей обменных курсов</param>
    /// <returns>Коллекция ExchangeRateDto</returns>
    public static IEnumerable<ExchangeRateDto> ToDto(this IEnumerable<ExchangeRate> exchangeRates) =>
        exchangeRates.Select(ToDto);
}