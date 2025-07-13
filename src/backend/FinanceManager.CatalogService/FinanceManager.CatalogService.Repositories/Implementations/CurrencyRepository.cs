using System.Linq.Expressions;
using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.Currencies;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.CatalogService.Repositories.Implementations;

public class CurrencyRepository(DatabaseContext context)
    : BaseRepository<Currency, CurrencyFilterDto>(context), ICurrencyRepository
{
    private readonly DbContext _context = context;

    private protected override IQueryable<Currency> SetFilters(CurrencyFilterDto filter, IQueryable<Currency> query)
    {
        if (filter.NameContains != null)
        {
            query = filter.NameContains.Length > 0
                ? query.Where(c => c.Name.Contains(filter.NameContains))
                : query.Where(c => string.Equals(c.Name, string.Empty));
        }

        if (filter.CharCode != null)
        {
            query = filter.CharCode.Length > 0
                ? query.Where(c => c.CharCode.Contains(filter.CharCode))
                : query.Where(c => string.Equals(c.CharCode, string.Empty));
        }

        if (filter.NumCode != null)
        {
            query = filter.NumCode.Length > 0
                ? query.Where(c => c.NumCode.Contains(filter.NumCode))
                : query.Where(c => string.Equals(c.NumCode, string.Empty));
        }

        return query;
    }

    public async Task<int> InitializeAsync(IEnumerable<Currency> entities,
        CancellationToken cancellationToken = default)
    {
        if (!await Entities.AnyAsync(cancellationToken))
        {
            await Entities.AddRangeAsync(entities, cancellationToken);
            return await _context.SaveChangesAsync(cancellationToken);
        }

        var query = Entities.AsQueryable();
        foreach (var entity in entities)
        {
            if (!await query.AnyAsync(
                    c => string.Equals(c.Name, entity.Name,
                             StringComparison.InvariantCultureIgnoreCase)
                         && string.Equals(c.CharCode, entity.CharCode,
                             StringComparison.InvariantCultureIgnoreCase)
                         && string.Equals(c.NumCode, entity.NumCode,
                             StringComparison.InvariantCultureIgnoreCase)
                    , cancellationToken))
            {
                await Entities.AddAsync(entity, cancellationToken);
            }
        }

        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ICollection<Currency>> GetAllOrderedByNameAsync(bool includeDeleted = false,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        return await GetAllOrderedByAsync(c => c.Name, includeDeleted, ascending, cancellationToken);
    }

    public async Task<ICollection<Currency>> GetAllOrderedByCharCodeAsync(bool includeDeleted = false,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        return await GetAllOrderedByAsync(c => c.CharCode, includeDeleted, ascending, cancellationToken);
    }

    public async Task<bool> IsCharCodeUniqueAsync(string charCode, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsQueryable();
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        return !await query.AnyAsync(
            c => string.Equals(c.CharCode, charCode, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken: cancellationToken);
    }

    public async Task<bool> IsNumCodeUniqueAsync(string numCode, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsQueryable();
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        return !await query.AnyAsync(
            c => string.Equals(c.NumCode, numCode, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken: cancellationToken);
    }

    public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsQueryable();
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        return !await query.AnyAsync(
            c => string.Equals(c.Name, name, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken: cancellationToken);
    }

    public Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Guid id, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private async Task<ICollection<Currency>> GetAllOrderedByAsync(Expression<Func<Currency, string>> selector,
        bool includeDeleted = false, bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsNoTracking();
        if (!includeDeleted)
            query = query.Where(c => !c.IsDeleted);
        if (ascending)
            return await query.OrderBy(selector).ToListAsync(cancellationToken);
        return await query.OrderByDescending(selector).ToListAsync(cancellationToken);
    }
}