using FinanceManager.CatalogService.Contracts.DTOs.Currencies;
using FluentResults;

namespace FinanceManager.CatalogService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы со справочником валют
/// </summary>
public interface ICurrencyService
{
    /// <summary>
    /// Получает валюту по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с данными валюты или ошибкой</returns>
    Task<Result<CurrencyDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список валют с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком валют или ошибкой</returns>
    Task<Result<ICollection<CurrencyDto>>> GetPagedAsync(
        CurrencyFilterDto filter, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Создает новую валюту
    /// </summary>
    /// <param name="createDto">Данные для создания валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданной валютой или ошибкой</returns>
    Task<Result<CurrencyDto>> CreateAsync(
        CreateCurrencyDto createDto, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую валюту
    /// </summary>
    /// <param name="updateDto">Данные для обновления валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленной валютой или ошибкой</returns>
    Task<Result<CurrencyDto>> UpdateAsync(
        UpdateCurrencyDto updateDto, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Выполняет мягкое удаление валюты
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Восстанавливает мягко удаленную валюту
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> RestoreAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Выполняет полное удаление валюты из базы данных
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}