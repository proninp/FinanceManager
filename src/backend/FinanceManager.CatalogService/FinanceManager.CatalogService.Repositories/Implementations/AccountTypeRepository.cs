using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.AccountTypes;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.CatalogService.Repositories.Implementations;

/// <summary>
/// Репозиторий для управления сущностями <see cref="AccountType"/>.
/// Предоставляет методы для фильтрации, получения всех типов счетов, проверки уникальности кода, проверки существования по коду и проверки возможности удаления.
/// </summary>
public class AccountTypeRepository(DatabaseContext context)
    : BaseRepository<AccountType, AccountTypeFilterDto>(context), IAccountTypeRepository
{
    private readonly DatabaseContext _context = context;

    /// <summary>
    /// Применяет фильтры к запросу <see cref="AccountType"/> на основе переданного <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">DTO фильтра с критериями фильтрации.</param>
    /// <param name="query">Исходный запрос для применения фильтров.</param>
    /// <returns>Отфильтрованный <see cref="IQueryable{AccountType}"/>.</returns>
    private protected override IQueryable<AccountType> SetFilters(AccountTypeFilterDto filter,
        IQueryable<AccountType> query)
    {
        if (filter.Code != null)
        {
            query = filter.Code.Length > 0
                ? query.Where(a => a.Code.Contains(filter.Code))
                : query.Where(a => string.Equals(a.Code, string.Empty));
        }

        if (filter.DescriptionContains != null)
        {
            query = filter.DescriptionContains.Length > 0
                ? query.Where(a => a.Description.Contains(filter.DescriptionContains))
                : query.Where(a => string.Equals(a.Description, string.Empty));
        }

        return query;
    }

    /// <summary>
    /// Получает все сущности <see cref="AccountType"/>.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция типов счетов.</returns>
    public async Task<ICollection<AccountType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Entities.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Проверяет уникальность кода типа счета.
    /// </summary>
    /// <param name="code">Код для проверки.</param>
    /// <param name="excludeId">Необязательный идентификатор типа счета, который нужно исключить из проверки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если код уникален; иначе — false.</returns>
    public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = 
            Entities.Where(a => string.Equals(a.Code, code, StringComparison.InvariantCultureIgnoreCase));
        if (excludeId != null)
            query = query.Where(a => a.Id != excludeId);
        return !await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Проверяет существование типа счета по коду.
    /// </summary>
    /// <param name="code">Код типа счета.</param>
    /// <param name="includeDeleted">Включать ли удалённые типы счетов.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если тип счета существует; иначе — false.</returns>
    public async Task<bool> ExistsByCodeAsync(string code, bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.Where(a => a.Code == code);
        if (!includeDeleted)
            query = query.Where(a => !a.IsDeleted);
        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Определяет, может ли тип счета быть удалён (т.е. не используется ни одним счетом).
    /// </summary>
    /// <param name="id">Идентификатор типа счета для проверки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если тип счета можно удалить; иначе — false.</returns>
    public async Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return !await _context.Accounts.AnyAsync(a => a.AccountTypeId == id, cancellationToken);
    }
}