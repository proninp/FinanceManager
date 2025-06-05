namespace FinanceManager.CatalogService.Contracts.DTOs.ExchangeRates;

/// <summary>
/// DTO для фильтрации и пагинации списка обменных курсов
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="CurrencyId">Идентификатор валюты</param>
/// <param name="RateDate">Дата курса</param>
/// <param name="Rate">Значение курса</param>
public record ExchangeRateFilterDto(
    int ItemsPerPage,
    int Page,
    Guid? CurrencyId,
    DateTime? RateDate,
    decimal? Rate
);