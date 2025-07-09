using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.Accounts;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.CatalogService.Repositories.Implementations;

/// <summary>
/// Репозиторий для работы со счетами.
/// Предоставляет методы фильтрации, получения количества счетов по владельцу, проверки наличия и получения счета по умолчанию.
/// </summary>
public class AccountRepository(DatabaseContext context)
    : BaseRepository<Account, AccountFilterDto>(context), IAccountRepository
{
    private readonly DatabaseContext _context = context;

    /// <summary>
    /// Применяет фильтры к запросу счетов.
    /// </summary>
    /// <param name="filter">Фильтр счетов.</param>
    /// <param name="query">Исходный запрос.</param>
    /// <returns>Запрос с применёнными фильтрами.</returns>
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
        if (filter.NameContains != null)
        {
            query = filter.NameContains.Length > 0
                ? query.Where(a => a.Name.Contains(filter.NameContains))
                : query.Where(a => string.Equals(a.Name, string.Empty));
        }
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

    /// <summary>
    /// Получает количество счетов по идентификатору владельца справочника с учётом архивных и удалённых.
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца справочника.</param>
    /// <param name="includeArchived">Включать архивные счета.</param>
    /// <param name="includeDeleted">Включать удалённые счета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Количество счетов.</returns>
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

    /// <summary>
    /// Проверяет, существует ли счет по умолчанию для владельца справочника (с возможностью исключения определённого счета).
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца справочника.</param>
    /// <param name="excludeId">Идентификатор счета, который нужно исключить из проверки (опционально).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если счет по умолчанию существует, иначе false.</returns>
    public async Task<bool> HasDefaultAccountAsync(Guid registryHolderId, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsQueryable();
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);
        return await query.AnyAsync(a => a.RegistryHolderId == registryHolderId && a.IsDefault, cancellationToken);
    }

    /// <summary>
    /// Получает счет по умолчанию для владельца справочника.
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца справочника.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Счет по умолчанию или null, если не найден.</returns>
    public async Task<Account?> GetDefaultAccountAsync(Guid registryHolderId,
        CancellationToken cancellationToken = default)
    {
        return await Entities.FirstOrDefaultAsync(a => a.RegistryHolderId == registryHolderId && a.IsDefault,
            cancellationToken);
    }
}