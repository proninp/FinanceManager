using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.Banks;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FinanceManager.CatalogService.Repositories.Implementations;

/// <summary>
/// Репозиторий для управления сущностями <see cref="Bank"/>.
/// Предоставляет методы для фильтрации, инициализации, проверки уникальности, получения связанных данных и проверки возможности удаления.
/// </summary>
public class BankRepository(DatabaseContext context, ILogger logger)
    : BaseRepository<Bank, BankFilterDto>(context, logger), IBankRepository
{
    private readonly DatabaseContext _context = context;

    /// <summary>
    /// Включает связанные сущности для <see cref="Bank"/> (например, страну).
    /// </summary>
    /// <param name="query">Исходный запрос.</param>
    /// <returns>Запрос с включёнными связанными сущностями.</returns>
    private protected override IQueryable<Bank> IncludeRelatedEntities(IQueryable<Bank> query)
    {
        return query
            .Include(b => b.Country);
    }

    /// <summary>
    /// Применяет фильтры к запросу <see cref="Bank"/> на основе переданного <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">DTO фильтра с критериями фильтрации.</param>
    /// <param name="query">Исходный запрос для применения фильтров.</param>
    /// <returns>Отфильтрованный <see cref="IQueryable{Bank}"/>.</returns>
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

    /// <summary>
    /// Инициализирует репозиторий набором сущностей <see cref="Bank"/>, если они ещё не существуют.
    /// Добавляет только уникальные банки по имени и стране.
    /// </summary>
    /// <param name="entities">Коллекция банков для инициализации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Количество записей, сохранённых в базе данных.</returns>
    public async Task<int> InitializeAsync(IEnumerable<Bank> entities, CancellationToken cancellationToken = default)
    {
        if (!await Entities.AnyAsync(cancellationToken))
        {
            await Entities.AddRangeAsync(entities, cancellationToken);
            return await _context.CommitAsync(cancellationToken);
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

        return await _context.CommitAsync(cancellationToken);
    }

    /// <summary>
    /// Получает все сущности <see cref="Bank"/> с возможностью включения связанных данных.
    /// </summary>
    /// <param name="includeRelated">Включать ли связанные сущности.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция банков.</returns>
    public async Task<ICollection<Bank>> GetAllAsync(bool includeRelated = true,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsNoTracking();
        if (includeRelated)
            query = IncludeRelatedEntities(query);
        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Проверяет уникальность имени банка в рамках страны.
    /// </summary>
    /// <param name="name">Имя банка для проверки.</param>
    /// <param name="countryId">Идентификатор страны.</param>
    /// <param name="excludeId">Необязательный идентификатор банка, который нужно исключить из проверки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если имя уникально в рамках страны; иначе — false.</returns>
    public async Task<bool> IsNameUniqueByCountryAsync(string name, Guid countryId, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.Where(b => b.CountryId == countryId);
        return await IsUniqueAsync(query,
            predicate: b => string.Equals(b.Name, name, StringComparison.InvariantCultureIgnoreCase),
            excludeId, cancellationToken);
    }

    /// <summary>
    /// Получает количество счетов, связанных с банком.
    /// </summary>
    /// <param name="bankId">Идентификатор банка.</param>
    /// <param name="includeArchivedAccounts">Включать ли архивные счета.</param>
    /// <param name="includeDeletedAccounts">Включать ли удалённые счета.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Количество счетов.</returns>
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

    /// <summary>
    /// Определяет, может ли банк быть удалён (т.е. не используется ни одним счётом).
    /// </summary>
    /// <param name="bankId">Идентификатор банка для проверки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если банк можно удалить; иначе — false.</returns>
    public async Task<bool> CanBeDeletedAsync(Guid bankId, CancellationToken cancellationToken = default)
    {
        return !await _context.Accounts.AnyAsync(a => a.BankId == bankId, cancellationToken);
    }
}