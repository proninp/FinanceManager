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
    
    /// <summary>
    /// Получает счет по умолчанию для владельца реестра
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Счет по умолчанию</returns>
    Task<Account?> GetDefaultAccountAsync(
        Guid registryHolderId,
        CancellationToken cancellationToken = default);
}