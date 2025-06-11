using FinanceManager.CatalogService.Contracts.DTOs.Countries;
using FluentResults;

namespace FinanceManager.CatalogService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы со справочником стран
/// </summary>
public interface ICountryService
{
    /// <summary>
    /// Получает страну по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор страны</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с данными страны или ошибкой</returns>
    Task<Result<CountryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает список стран с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком стран или ошибкой</returns>
    Task<Result<IEnumerable<CountryDto>>> GetPagedAsync(
        CountryFilterDto filter, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Создает новую страну
    /// </summary>
    /// <param name="createDto">Данные для создания страны</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданной страной или ошибкой</returns>
    Task<Result<CountryDto>> CreateAsync(
        CreateCountryDto createDto, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновляет существующую страну
    /// </summary>
    /// <param name="updateDto">Данные для обновления страны</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленной страной или ошибкой</returns>
    Task<Result<CountryDto>> UpdateAsync(
        UpdateCountryDto updateDto, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет страну
    /// </summary>
    /// <param name="id">Идентификатор страны</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}