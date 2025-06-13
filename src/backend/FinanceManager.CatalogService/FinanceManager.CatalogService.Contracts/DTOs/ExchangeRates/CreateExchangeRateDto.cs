using FinanceManager.CatalogService.Domain.Entities;

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

/// <summary>
/// Методы-расширения для преобразования CreateExchangeRateDto в ExchangeRate
/// </summary>
public static class CreateExchangeRateDtoExtensions
{
    /// <summary>
    /// Преобразует DTO создания обменного курса в сущность ExchangeRate
    /// </summary>
    /// <param name="dto">DTO для создания обменного курса</param>
    /// <returns>Экземпляр ExchangeRate</returns>
    public static ExchangeRate ToExchangeRate(this CreateExchangeRateDto dto) =>
        new ExchangeRate(dto.RateDate, dto.CurrencyId, dto.Rate);
}