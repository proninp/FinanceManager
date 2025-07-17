using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Categories;
using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Abstractions.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с категориями доходов и расходов
/// </summary>
public interface ICategoryRepository : IBaseRepository<Category, CategoryFilterDto>
{
    /// <summary>
    /// Получает все категории пользователя
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="includeRelated">Включать ли связанные сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список категорий пользователя</returns>
    Task<ICollection<Category>> GetByRegistryHolderIdAsync(
        Guid registryHolderId,
        bool includeRelated = true,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверяет уникальность названия категории у пользователя
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="name">Название категории</param>
    /// <param name="parentId">Идентификатор родительской категории (null для корневых)</param>
    /// <param name="excludeId">Исключить категорию с данным ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если название уникально на данном уровне</returns>
    Task<bool> IsNameUniqueInScopeAsync(
        Guid registryHolderId,
        string name,
        Guid? parentId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверяет, не создает ли изменение родительской категории циклическую зависимость
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <param name="newParentId">Новый идентификатор родительской категории</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если изменение безопасно (не создает циклов)</returns>
    Task<bool> IsParentChangeValidAsync(
        Guid categoryId,
        Guid? newParentId,
        CancellationToken cancellationToken = default);
}