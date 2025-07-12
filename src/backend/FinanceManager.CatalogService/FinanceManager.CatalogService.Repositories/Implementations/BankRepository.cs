using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.Banks;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.CatalogService.Repositories.Implementations;

public class BankRepository(DatabaseContext context) : BaseRepository<Bank, BankFilterDto>(context), IBankRepository
{
    private readonly DatabaseContext _context = context;

    private protected override IQueryable<Bank> IncludeRelatedEntities(IQueryable<Bank> query)
    {
        return query
            .Include(b => b.Country);
    }

    private protected override IQueryable<Bank> SetFilters(BankFilterDto filter, IQueryable<Bank> query)
    {
        if (filter.CountryId.HasValue)
            query = query.Where(b => b.CountryId == filter.CountryId.Value);
        if (filter.NameContains != null)
        {
            query = filter.NameContains.Length > 0
                ? query.Where(b => b.Name.Contains(filter.NameContains))
                : query.Where(b => string.Equals(b.Name, string.Empty));
        }

        return query;
    }

    public async Task<int> InitializeAsync(IEnumerable<Bank> entities, CancellationToken cancellationToken = default)
    {
        if (await Entities.AnyAsync(cancellationToken))
        {
            await Entities.AddRangeAsync(entities, cancellationToken);
            return await _context.SaveChangesAsync(cancellationToken);
        }

        var query = Entities.AsQueryable();
        foreach (var entity in entities)
        {
            if (!await query.AnyAsync(
                    b => b.CountryId == entity.CountryId && string.Equals(b.Name, entity.Name,
                        StringComparison.InvariantCultureIgnoreCase), cancellationToken))
            {
                await Entities.AddAsync(entity, cancellationToken);
            }
        }

        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ICollection<Bank>> GetAllAsync(bool includeRelated = true,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsNoTracking();
        if (includeRelated)
            query = IncludeRelatedEntities(query);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> IsNameUniqueByCountryAsync(string name, Guid countryId, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.Where(b => b.CountryId == countryId);
        if (excludeId.HasValue)
            query = query.Where(b => b.Id != excludeId.Value);

        return await query.AnyAsync(b => string.Equals(b.Name, name, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken: cancellationToken);
    }

    public async Task<int> GetAccountsCountAsync(Guid bankId, bool includeArchivedAccounts = false,
        bool includeDeletedAccounts = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Accounts.Where(b => b.BankId == bankId);
        if (!includeArchivedAccounts)
            query = query.Where(a => a.IsArchived == false);
        if (!includeDeletedAccounts)
            query = query.Where(a => a.IsDeleted == false);
        
        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> CanBeDeletedAsync(Guid bankId, CancellationToken cancellationToken = default)
    {
        return await _context.Accounts.AnyAsync(a => a.BankId == bankId, cancellationToken);
    }
}