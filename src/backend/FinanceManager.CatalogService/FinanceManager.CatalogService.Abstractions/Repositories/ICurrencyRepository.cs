using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Currencies;
using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Abstractions.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с валютами
/// </summary>
public interface ICurrencyRepository : IBaseRepository<Currency, CurrencyFilterDto>, ISoftDeletableRepository<Currency>,
    IInitializerRepository<Currency>
{
    /// <summary>
    /// Получает все валюты, отсортированные по названию
    /// </summary>
    /// <param name="includeDeleted">Включать ли удаленные валюты</param>
    /// <param name="ascending">Направление сортировки</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список валют, отсортированный по названию</returns>
    Task<ICollection<Currency>> GetAllOrderedByNameAsync(bool includeDeleted = false, bool ascending = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает валюты, отсортированные по символьному коду
    /// </summary>
    /// <param name="includeDeleted">Включать ли удаленные валюты</param>
    /// <param name="ascending">Направление сортировки</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список валют, отсортированный по коду</returns>
    Task<ICollection<Currency>> GetAllOrderedByCharCodeAsync(bool includeDeleted = false, bool ascending = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет уникальность символьного кода валюты
    /// </summary>
    /// <param name="charCode">Символьный код валюты</param>
    /// <param name="excludeId">Исключить валюту с данным ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если код уникален</returns>
    Task<bool> IsCharCodeUniqueAsync(string charCode, Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет уникальность числового кода валюты
    /// </summary>
    /// <param name="numCode">Числовой код валюты</param>
    /// <param name="excludeId">Исключить валюту с данным ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если код уникален</returns>
    Task<bool> IsNumCodeUniqueAsync(string numCode, Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет уникальность названия валюты
    /// </summary>
    /// <param name="name">Название валюты</param>
    /// <param name="excludeId">Исключить валюту с данным ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если название уникально</returns>
    Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, используется ли валюта в счетах
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если валюта используется в счетах</returns>
    Task<bool> IsUsedInAccountsAsync(Guid currencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, есть ли для валюты обменные курсы
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если для валюты есть курсы</returns>
    Task<bool> HasExchangeRatesAsync(Guid currencyId, CancellationToken cancellationToken = default);
}