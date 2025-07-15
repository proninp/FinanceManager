using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.Categories;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.CatalogService.Repositories.Implementations;

/// <summary>
/// Репозиторий для работы с категориями.
/// Предоставляет методы фильтрации, проверки уникальности имени, проверки валидности смены родителя и получения категорий по владельцу.
/// </summary>
public class CategoryRepository(DatabaseContext context)
    : BaseRepository<Category, CategoryFilterDto>(context), ICategoryRepository
{
    private readonly DatabaseContext _context = context;

    /// <summary>
    /// Включает связанные сущности RegistryHolder и Parent для категории.
    /// </summary>
    private protected override IQueryable<Category> IncludeRelatedEntities(IQueryable<Category> query)
    {
        return query
            .Include(c => c.RegistryHolder)
            .Include(c => c.Parent);
    }

    /// <summary>
    /// Применяет фильтры к запросу категорий.
    /// </summary>
    /// <param name="filter">Фильтр категорий.</param>
    /// <param name="query">Исходный запрос.</param>
    /// <returns>Запрос с применёнными фильтрами.</returns>
    private protected override IQueryable<Category> SetFilters(CategoryFilterDto filter, IQueryable<Category> query)
    {
        if (filter.RegistryHolderId.HasValue)
            query = query.Where(c => c.RegistryHolderId == filter.RegistryHolderId);
        if (!string.IsNullOrWhiteSpace(filter.NameContains))
            query = query.Where(c => c.Name.Contains(filter.NameContains));
        if (filter.Income.HasValue)
            query = query.Where(c => c.Income == filter.Income.Value);
        if (filter.Expense.HasValue)
            query = query.Where(c => c.Expense == filter.Expense.Value);
        if (filter.ParentId.HasValue)
            query = query.Where(c => c.ParentId == filter.ParentId);
        return query;
    }

    /// <summary>
    /// Получает коллекцию категорий по идентификатору владельца справочника.
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца справочника.</param>
    /// <param name="includeRelated">Включать связанные сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция категорий.</returns>
    public async Task<ICollection<Category>> GetByRegistryHolderIdAsync(Guid registryHolderId,
        bool includeRelated = true,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsQueryable();

        query = query.Where(c => c.RegistryHolderId == registryHolderId);

        if (includeRelated)
            query = IncludeRelatedEntities(query);

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Проверяет уникальность имени категории в рамках владельца и родителя.
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца справочника.</param>
    /// <param name="name">Имя категории.</param>
    /// <param name="parentId">Идентификатор родительской категории.</param>
    /// <param name="excludeId">Идентификатор категории, которую нужно исключить из проверки (опционально).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если имя уникально, иначе false.</returns>
    public async Task<bool> IsNameUniqueInScopeAsync(Guid registryHolderId, string name, Guid? parentId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsNoTracking();
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        query = parentId.HasValue
            ? query.Where(c => c.ParentId == parentId.Value)
            : query.Where(c => c.ParentId == null);
        var isAny = await query.AnyAsync(c => c.RegistryHolderId == registryHolderId && c.Name == name,
            cancellationToken);
        return isAny;
    }

    /// <summary>
    /// Проверяет, допустима ли смена родителя для категории (нет циклических зависимостей).
    /// </summary>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <param name="newParentId">Идентификатор нового родителя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если смена допустима, иначе false.</returns>
    public async Task<bool> IsParentChangeValidAsync(Guid categoryId, Guid? newParentId,
        CancellationToken cancellationToken = default)
    {
        if (newParentId.HasValue)
            return !await CheckCycleAsync(categoryId, newParentId.Value, cancellationToken);
        return true;
    }

    /// <summary>
    /// Проверяет наличие циклической зависимости при смене родителя категории.
    /// </summary>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <param name="newParentId">Идентификатор нового родителя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если цикл найден, иначе false.</returns>
    private async Task<bool> CheckCycleAsync(Guid categoryId, Guid newParentId,
        CancellationToken cancellationToken = default)
    {
        const string categoryIdParamName = "@categoryId";
        const string newParentIdParamName = "@newParentId";
        const string sqlQuery = $"""
                                WITH RECURSIVE category_tree AS (
                                    SELECT id, parent_id, ARRAY[id] AS path, false AS is_cycle
                                    FROM categories
                                    WHERE id = {newParentIdParamName}
                                    UNION ALL
                                    SELECT c.id, c.parent_id, path || c.id,
                                        c.id = ANY(path) OR is_cycle
                                    FROM categories c
                                    JOIN category_tree ct ON c.id = ct.parent_id
                                )
                                SELECT EXISTS (SELECT 1 FROM category_tree WHERE id = {categoryIdParamName} OR is_cycle);
                                """;
        
        await using var connection = _context.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = sqlQuery;
        
        var categoryIdParam = command.CreateParameter();
        categoryIdParam.ParameterName = categoryIdParamName;
        categoryIdParam.Value = categoryId;
        command.Parameters.Add(categoryIdParam);
        
        var newParentIdParam = command.CreateParameter();
        newParentIdParam.ParameterName = newParentIdParamName;
        newParentIdParam.Value = newParentId;
        command.Parameters.Add(newParentIdParam);
        
        var result = await command.ExecuteScalarAsync(cancellationToken);
        
        return result is bool and true;
    }
}
