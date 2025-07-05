using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Abstractions;
using FinanceManager.CatalogService.Domain.Abstractions;
using FinanceManager.CatalogService.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.CatalogService.Repositories.Abstractions;

public abstract class BaseRepository<T, TFilterDto>(DatabaseContext context) : IBaseRepository<T, TFilterDto>
    where T : IdentityModel
    where TFilterDto : BasePaginationDto
{
    private protected readonly DbSet<T> Entities = context.Set<T>();
    
    public async Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Entities
            .AsNoTracking()
            .AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, bool includeRelated = true, bool disableTracking = false,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsQueryable();
        if (disableTracking)
            query = query.AsNoTrackingWithIdentityResolution();

        if (includeRelated)
            IncludeRelatedEntities(query);
        
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    private protected abstract void IncludeRelatedEntities(IQueryable<T> entities);

    public async Task<ICollection<T>> GetPagedAsync(TFilterDto filter, bool includeRelated = true,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsNoTracking();
        
        SetFilters(query);
        
        query = query.Skip(filter.Skip).Take(filter.Take);
        
        return await query.ToListAsync(cancellationToken);
    }
    
    private protected abstract void SetFilters(IQueryable<T> entities);

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var addedEntity = await Entities.AddAsync(entity, cancellationToken);
        return addedEntity.Entity;
    }

    public T Update(T entity)
    {
        return Entities.Update(entity).Entity;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await Entities
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        return result > 0;
    }
}