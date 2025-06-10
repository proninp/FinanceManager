using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Contracts.DTOs.AccountTypes;
using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Abstractions.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с типами банковских счетов
/// </summary>
public interface IAccountTypeRepository : IBaseRepository<AccountType, AccountTypeFilterDto>
{
    /// <summary>
    /// Получает все типы счетов, включая удаленные
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список всех типов счетов</returns>
    Task<IEnumerable<AccountType>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверяет уникальность кода типа счета
    /// </summary>
    /// <param name="code">Код типа счета</param>
    /// <param name="excludeId">Исключить тип счета с данным ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если код уникален</returns>
    Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет существование типа счета по коду
    /// </summary>
    /// <param name="code">Код типа счета</param>
    /// <param name="includeDeleted">Включать ли удаленные записи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если тип счета с таким кодом существует</returns>
    Task<bool> ExistsByCodeAsync(string code, bool includeDeleted = false, CancellationToken cancellationToken = default);
}