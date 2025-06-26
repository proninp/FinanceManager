using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Banks;
using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Abstractions.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с банками
/// </summary>
public interface IBankRepository : IBaseRepository<Bank, BankFilterDto>, IInitializerRepository<Bank>
{
    /// <summary>
    /// Получает все банки
    /// </summary>
    /// <param name="includeRelated">Включать ли связанные сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список всех банков</returns>
    Task<ICollection<Bank>> GetAllAsync(
        bool includeRelated = true, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверяет уникальность названия банка в рамках страны
    /// </summary>
    /// <param name="name">Название банка</param>
    /// <param name="countryId">Id страны банка</param>
    /// <param name="excludeId">Исключить банк с данным ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если название уникально глобально</returns>
    Task<bool> IsNameUniqueByCountryAsync(
        string name,
        Guid countryId,
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает количество счетов, использующих данный банк
    /// </summary>
    /// <param name="bankId">Идентификатор банка</param>
    /// <param name="includeArchivedAccounts">Включать ли архивированные счета</param>
    /// <param name="includeDeletedAccounts">Включать ли удаленные счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество счетов данного банка</returns>
    Task<int> GetAccountsCountAsync(
        Guid bankId, 
        bool includeArchivedAccounts = false, 
        bool includeDeletedAccounts = false, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверяет, можно ли удалить банк (не используется ли он в активных счетах)
    /// </summary>
    /// <param name="bankId">Идентификатор банка</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если банк можно удалить</returns>
    Task<bool> CanBeDeletedAsync(Guid bankId, CancellationToken cancellationToken = default);
}