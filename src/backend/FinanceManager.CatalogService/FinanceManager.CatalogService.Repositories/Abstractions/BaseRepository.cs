using System.Linq.Expressions;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Abstractions;
using FinanceManager.CatalogService.Domain.Abstractions;
using FinanceManager.CatalogService.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FinanceManager.CatalogService.Repositories.Abstractions;

/// <summary>
/// Базовый репозиторий для работы с сущностями, поддерживающий пагинацию и фильтрацию.
/// </summary>
/// <typeparam name="T">Тип сущности.</typeparam>
/// <typeparam name="TFilterDto">Тип DTO фильтра для пагинации.</typeparam>
public abstract class BaseRepository<T, TFilterDto>(DatabaseContext context, ILogger logger)
    : IBaseRepository<T, TFilterDto>
    where T : IdentityModel
    where TFilterDto : BasePaginationDto
{
    private protected readonly DbSet<T> Entities = context.Set<T>();

    /// <summary>
    /// Проверяет, является ли сущность пустой
    /// </summary>
    /// <param name="cancellationToken">Токен отмены для прерывания операции</param>
    /// <returns>
    /// Задача, которая завершается с результатом:
    /// <c>true</c> - если сущность пуста;
    /// <c>false</c> - если содержит элементы
    /// </returns>
    public async Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default)
    {
        logger.Debug("Проверка на пустоту сущности {EntityType}", typeof(T).Name);
        var isEmpty = !await Entities.AnyAsync(cancellationToken);
        logger.Debug("Сущность {EntityType} {ExistsResult}", typeof(T).Name,
            isEmpty ? "не найдена" : "найдена");
        return isEmpty;
    }

    /// <summary>
    /// Проверяет, существует ли сущность с указанным идентификатором.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если сущность существует, иначе false.</returns>
    public async Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Debug("Проверка существования сущности {EntityType} с Id: {Id}", typeof(T).Name, id);
        var exists = await Entities
            .AnyAsync(e => e.Id == id, cancellationToken);
        logger.Debug("Сущность {EntityType} с Id {Id} {ExistsResult}", typeof(T).Name, id,
            exists ? "найдена" : "не найдена");
        return exists;
    }

    /// <summary>
    /// Получает сущность по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="includeRelated">Включать связанные сущности.</param>
    /// <param name="disableTracking">Отключить отслеживание изменений.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сущность или null, если не найдена.</returns>
    public async Task<T?> GetByIdAsync(Guid id, bool includeRelated = true, bool disableTracking = false,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Получение сущности {EntityType} по Id: {Id}", typeof(T).Name, id);
        var query = Entities.AsQueryable();
        if (disableTracking)
            query = query.AsNoTrackingWithIdentityResolution();

        if (includeRelated)
            query = IncludeRelatedEntities(query);

        var entity = await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is null)
        {
            logger.Warning("Сущность {EntityType} с Id {Id} не найдена", typeof(T).Name, id);
        }
        else
        {
            logger.Information("Сущность {EntityType} с Id {Id} успешно получена", typeof(T).Name, id);
        }

        return entity;
    }

    /// <summary>
    /// Включает связанные сущности в запрос.
    /// </summary>
    /// <param name="query">Исходный запрос.</param>
    /// <returns>Запрос с включёнными связанными сущностями.</returns>
    private protected virtual IQueryable<T> IncludeRelatedEntities(IQueryable<T> query) =>
        query;

    /// <summary>
    /// Получает страницу сущностей с применением фильтрации и пагинации.
    /// </summary>
    /// <param name="filter">Фильтр и параметры пагинации.</param>
    /// <param name="includeRelated">Включать связанные сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция сущностей.</returns>
    public async Task<ICollection<T>> GetPagedAsync(TFilterDto filter, bool includeRelated = true,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Получение страницы сущностей {EntityType}. Страница: {Page}, Размер: {PageSize}",
            typeof(T).Name, filter.Page, filter.ItemsPerPage);

        var query = Entities.AsNoTracking();
        query = SetFilters(filter, query);
        query = query.Skip(filter.Skip).Take(filter.Take);
        var entities = await query.ToListAsync(cancellationToken);

        logger.Information("Получено {Count} сущностей {EntityType} на странице {Page}",
            entities.Count, typeof(T).Name, filter.Page);

        return entities;
    }

    /// <summary>
    /// Применяет фильтры к запросу.
    /// </summary>
    /// <param name="filter">Фильтр.</param>
    /// <param name="query">Исходный запрос.</param>
    /// <returns>Запрос с применёнными фильтрами.</returns>
    private protected abstract IQueryable<T> SetFilters(TFilterDto filter, IQueryable<T> query);

    /// <summary>
    /// Добавляет новую сущность в набор.
    /// </summary>
    /// <param name="entity">Добавляемая сущность.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Добавленная сущность.</returns>
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        logger.Information("Добавление новой сущности {EntityType} с Id: {Id}", typeof(T).Name, entity.Id);
        var addedEntity = await Entities.AddAsync(entity, cancellationToken);
        logger.Information("Сущность {EntityType} с Id {Id} подготовлена к сохранению", typeof(T).Name, entity.Id);
        return addedEntity.Entity;
    }

    /// <summary>
    /// Обновляет сущность.
    /// </summary>
    /// <param name="entity">Обновляемая сущность.</param>
    /// <returns>Обновлённая сущность.</returns>
    public T Update(T entity)
    {
        logger.Information("Обновление сущности {EntityType} с Id: {Id}", typeof(T).Name, entity.Id);
        var updatedEntity = Entities.Update(entity).Entity;
        logger.Information("Сущность {EntityType} с Id {Id} подготовлена к обновлению", typeof(T).Name, entity.Id);
        return updatedEntity;
    }

    /// <summary>
    /// Удаляет сущность по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если сущность была удалена, иначе false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Удаление сущности {EntityType} с Id: {Id}", typeof(T).Name, id);
        
        var result = await Entities
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        if (result > 0)
        {
            logger.Information("Сущность {EntityType} с Id {Id} успешно удалена", typeof(T).Name, id);
        }
        else
        {
            logger.Warning("Сущность {EntityType} с Id {Id} не была удалена (возможно, не найдена)", typeof(T).Name, id);
        }
        return result > 0;
    }

    /// <summary>
    /// Проверяет, является ли запись уникальной в соответствии с заданным предикатом.
    /// </summary>
    /// <param name="query">Запрос, к которому применяется проверка.</param>
    /// <param name="predicate">Условие, определяющее уникальность записи.</param>
    /// <param name="excludeId">
    /// Идентификатор записи, которую нужно исключить из проверки (например, при обновлении записи).
    /// </param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Возвращает true, если запись уникальна (не существует других записей, удовлетворяющих предикату),
    /// иначе false.
    /// </returns>
    protected async Task<bool> IsUniqueAsync(IQueryable<T> query, Expression<Func<T, bool>> predicate,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        logger.Debug("Проверка уникальности сущности {EntityType}, исключая Id: {ExcludeId}", 
            typeof(T).Name, excludeId);
        if (excludeId.HasValue)
            query = query.Where(e => e.Id != excludeId.Value);

        var exists = await query.AnyAsync(predicate, cancellationToken: cancellationToken);
        
        logger.Debug("Результат проверки уникальности для {EntityType}: {IsUnique}", 
            typeof(T).Name, !exists ? "уникальна" : "не уникальна");
        
        return !exists;
    }
}