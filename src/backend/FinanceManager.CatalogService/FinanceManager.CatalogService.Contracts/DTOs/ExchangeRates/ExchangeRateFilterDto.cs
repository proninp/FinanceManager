using FinanceManager.CatalogService.Contracts.DTOs.Abstractions;

namespace FinanceManager.CatalogService.Contracts.DTOs.ExchangeRates;

/// <summary>
/// DTO для фильтрации и пагинации списка обменных курсов
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="CurrencyId">Идентификатор валюты</param>
/// <param name="DateFrom">Дата начала курса</param>
/// <param name="DateTo">Дата конца курса</param>
/// <param name="RateFrom">Значение курса от</param>
/// <param name="RateTo">Значение курса до</param>
public record ExchangeRateFilterDto(
    int ItemsPerPage,
    int Page,
    Guid? CurrencyId = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    decimal? RateFrom = null,
    decimal? RateTo = null
) : BasePaginationDto(ItemsPerPage, Page);