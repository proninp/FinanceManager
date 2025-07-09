using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.Accounts;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.CatalogService.Repositories.Implementations;

public class AccountRepository(DatabaseContext context)
    : BaseRepository<Account, AccountFilterDto>(context), IAccountRepository
{
    private readonly DatabaseContext _context = context;

    private protected override IQueryable<Account> SetFilters(AccountFilterDto filter, IQueryable<Account> query)
    {
        if (filter.RegistryHolderId.HasValue)
            query = query.Where(a => a.RegistryHolderId == filter.RegistryHolderId.Value);
        if (filter.AccountTypeId.HasValue)
            query = query.Where(a => a.AccountTypeId == filter.AccountTypeId.Value);
        if (filter.CurrencyId.HasValue)
            query = query.Where(a => a.CurrencyId == filter.CurrencyId.Value);
        if (filter.BankId.HasValue)
            query = query.Where(a => a.BankId == filter.BankId.Value);
        if (!string.IsNullOrWhiteSpace(filter.NameContains))
            query = query.Where(a => a.Name.Contains(filter.NameContains));
        if (filter.IsIncludeInBalance.HasValue)
            query = query.Where(a => a.IsIncludeInBalance == filter.IsIncludeInBalance.Value);
        if (filter.IsDefault.HasValue)
            query = query.Where(a => a.IsDefault == filter.IsDefault.Value);
        if (filter.IsArchived.HasValue)
            query = query.Where(a => a.IsArchived == filter.IsArchived.Value);
        if (filter.CreditLimitFrom.HasValue)
            query = query.Where(a => a.CreditLimit >= filter.CreditLimitFrom.Value);
        if (filter.CreditLimitTo.HasValue)
            query = query.Where(a => a.CreditLimit <= filter.CreditLimitTo.Value);
        return query;
    }

    public async Task<int> GetCountByRegistryHolderIdAsync(Guid registryHolderId, bool includeArchived = false,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        return await Entities
            .Where(a => a.RegistryHolderId == registryHolderId &&
                        (includeArchived || !a.IsArchived) &&
                        (includeDeleted || !a.IsDeleted))
            .CountAsync(cancellationToken);
    }

    public async Task<bool> HasDefaultAccountAsync(Guid registryHolderId, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsQueryable();
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);
        return await query.AnyAsync(a => a.RegistryHolderId == registryHolderId && a.IsDefault, cancellationToken);
    }

    public async Task<Account?> GetDefaultAccountAsync(Guid registryHolderId,
        CancellationToken cancellationToken = default)
    {
        return await Entities.FirstOrDefaultAsync(a => a.RegistryHolderId == registryHolderId && a.IsDefault,
            cancellationToken);
    }
}