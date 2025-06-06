using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Contracts.DTOs.ExchangeRates;
using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Abstractions.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с обменными курсами валют
/// </summary>
public interface IExchangeRateRepository : IBaseRepository<ExchangeRate, ExchangeRateFilterDto>
{
    /// <summary>
    /// Проверяет существование курса для валюты на дату
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="rateDate">Дата курса</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если курс существует</returns>
    Task<bool> ExistsForCurrencyAndDateAsync(Guid currencyId, DateTime rateDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, есть ли курсы на указанную дату
    /// </summary>
    /// <param name="rateDate">Дата курсов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если курсы на дату существуют</returns>
    Task<bool> ExistsForDateAsync(DateTime rateDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает дату последнего курса валюты
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Дата последнего курса или null</returns>
    Task<DateTime?> GetLastRateDateAsync(Guid currencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет несколько курсов валют
    /// </summary>
    /// <param name="exchangeRates">Список курсов для добавления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список добавленных курсов</returns>
    Task<IEnumerable<ExchangeRate>> AddRangeAsync(IEnumerable<ExchangeRate> exchangeRates,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет курсы валюты за период
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="dateFrom">Дата начала периода</param>
    /// <param name="dateTo">Дата окончания периода</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество удаленных курсов</returns>
    Task<int> DeleteByPeriodAsync(Guid currencyId, DateTime dateFrom, DateTime dateTo,
        CancellationToken cancellationToken = default);
}