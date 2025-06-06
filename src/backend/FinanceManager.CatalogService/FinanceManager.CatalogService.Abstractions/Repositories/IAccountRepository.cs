using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Accounts;
using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Abstractions.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с банковскими счетами
/// </summary>
public interface IAccountRepository :
    IBaseRepository<Account, AccountFilterDto>
{
    #region Специальные запросы по владельцу

    /// <summary>
    /// Получает общее количество счетов пользователя
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="includeArchived">Включать ли архивированные счета</param>
    /// <param name="includeDeleted">Включать ли удаленные записи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество счетов</returns>
    Task<int> GetCountByRegistryHolderIdAsync(
        Guid registryHolderId, 
        bool includeArchived = false, 
        bool includeDeleted = false, 
        CancellationToken cancellationToken = default);

    #endregion

    #region Проверки уникальности

    /// <summary>
    /// Проверяет уникальность названия счета у пользователя
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="name">Название счета</param>
    /// <param name="excludeId">Исключить счет с данным ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если название уникально</returns>
    Task<bool> IsNameUniqueAsync(
        Guid registryHolderId, 
        string name, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, есть ли у пользователя счет по умолчанию
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="excludeId">Исключить счет с данным ID</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если есть счет по умолчанию</returns>
    Task<bool> HasDefaultAccountAsync(
        Guid registryHolderId, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default);

    #endregion

    #region Операции изменения статуса

    /// <summary>
    /// Архивирует счет
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если счет был архивирован</returns>
    Task<bool> ArchiveAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Разархивирует счет
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если счет был разархивирован</returns>
    Task<bool> UnarchiveAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Устанавливает счет как счет по умолчанию (и снимает флаг с других счетов пользователя)
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если операция прошла успешно</returns>
    Task<bool> SetAsDefaultAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Снимает флаг "по умолчанию" со счета
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если операция прошла успешно</returns>
    Task<bool> UnsetAsDefaultAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion
}