using FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;
using FluentResults;

namespace FinanceManager.CatalogService.Abstractions.Services;

public interface IRegistryHolderService
{
    /// <summary>
    /// Получает владельца реестра по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор владельца реестра</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с данными владельца реестра или ошибкой</returns>
    Task<Result<RegistryHolderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает список владельцев реестра с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком владельцев реестра или ошибкой</returns>
    Task<Result<IEnumerable<RegistryHolderDto>>> GetPagedAsync(
        RegistryHolderFilterDto filter, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Создает нового владельца реестра
    /// </summary>
    /// <param name="createDto">Данные для создания владельца реестра</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданным владельцем реестра или ошибкой</returns>
    Task<Result<RegistryHolderDto>> CreateAsync(
        CreateRegistryHolderDto createDto, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновляет существующего владельца реестра
    /// </summary>
    /// <param name="updateDto">Данные для обновления владельца реестра</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленным владельцем реестра или ошибкой</returns>
    Task<Result<RegistryHolderDto>> UpdateAsync(
        UpdateRegistryHolderDto updateDto, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет владельца реестра
    /// </summary>
    /// <param name="id">Идентификатор владельца реестра</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}