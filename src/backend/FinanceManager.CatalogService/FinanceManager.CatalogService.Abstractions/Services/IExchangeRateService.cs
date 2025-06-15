using FinanceManager.CatalogService.Contracts.DTOs.ExchangeRates;
using FluentResults;

namespace FinanceManager.CatalogService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы с обменными курсами валют
/// </summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Получает обменный курс по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор курса</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с данными курса или ошибкой</returns>
    Task<Result<ExchangeRateDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список обменных курсов с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком курсов или ошибкой</returns>
    Task<Result<ICollection<ExchangeRateDto>>> GetPagedAsync(
        ExchangeRateFilterDto filter, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создает новый обменный курс
    /// </summary>
    /// <param name="createDto">Данные для создания курса</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданным курсом или ошибкой</returns>
    Task<Result<ExchangeRateDto>> CreateAsync(
        CreateExchangeRateDto createDto, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующий обменный курс
    /// </summary>
    /// <param name="updateDto">Данные для обновления курса</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленным курсом или ошибкой</returns>
    Task<Result<ExchangeRateDto>> UpdateAsync(
        UpdateExchangeRateDto updateDto, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет обменный курс
    /// </summary>
    /// <param name="id">Идентификатор курса</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет несколько курсов валют за один раз
    /// </summary>
    /// <param name="createDtos">Список курсов для добавления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком добавленных курсов или ошибкой</returns>
    Task<Result<IEnumerable<ExchangeRateDto>>> AddRangeAsync(
        IEnumerable<CreateExchangeRateDto> createDtos,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет существование курса для валюты на определенную дату
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="rateDate">Дата курса</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки существования</returns>
    Task<Result<bool>> ExistsForCurrencyAndDateAsync(
        Guid currencyId, 
        DateTime rateDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает дату последнего курса валюты
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с датой последнего курса или ошибкой</returns>
    Task<Result<DateTime?>> GetLastRateDateAsync(
        Guid currencyId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет курсы валюты за определенный период
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="dateFrom">Дата начала периода</param>
    /// <param name="dateTo">Дата окончания периода</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с количеством удаленных курсов или ошибкой</returns>
    Task<Result<int>> DeleteByPeriodAsync(
        Guid currencyId, 
        DateTime dateFrom, 
        DateTime dateTo,
        CancellationToken cancellationToken = default);
}