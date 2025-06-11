using FinanceManager.CatalogService.Contracts.DTOs.Accounts;
using FluentResults;

namespace FinanceManager.CatalogService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы с типами банковских счетов
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Получает счет по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>DTO счета или null, если не найден</returns>
    Task<Result<AccountDto>?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает список счетов с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком счетов или ошибкой</returns>
    Task<Result<IEnumerable<AccountDto>>> GetPagedAsync(
        AccountFilterDto filter, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает счет по умолчанию пользователя
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со счетом по умолчанию или ошибкой</returns>
    Task<Result<AccountDto?>> GetDefaultAccountAsync(
        Guid registryHolderId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Создает новый счет
    /// </summary>
    /// <param name="createDto">Данные для создания счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданным счетом или ошибкой</returns>
    Task<Result<AccountDto>> CreateAsync(
        CreateAccountDto createDto, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Обновляет существующий счет
    /// </summary>
    /// <param name="updateDto">Данные для обновления счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленным счетом или ошибкой</returns>
    Task<Result<AccountDto>> UpdateAsync(
        UpdateAccountDto updateDto, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Удаляет счет
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Архивирует счет
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> ArchiveAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Разархивирует счет
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> UnarchiveAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Устанавливает счет как счет по умолчанию
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> SetAsDefaultAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Снимает флаг "по умолчанию" со счета
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> UnsetAsDefaultAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверяет уникальность названия счета у пользователя
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="name">Название счета</param>
    /// <param name="excludeId">Исключить счет с данным ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки уникальности</returns>
    Task<Result<bool>> IsNameUniqueAsync(
        Guid registryHolderId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}