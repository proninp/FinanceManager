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
        logger.Information("Начинается инициализация сущности Банк");
        if (!await Entities.AnyAsync(cancellationToken))
        {
            logger.Debug("Таблица банков пуста, добавляем все");
            await Entities.AddRangeAsync(entities, cancellationToken);
            var result = await _context.CommitAsync(cancellationToken);
            logger.Information("Инициализация завершена, добавлено {AddedCount} банков", result);
            return result;
        }

        logger.Debug("Таблица банков содержит данные, проверяем уникальность");
        var query = Entities.AsQueryable();
        var addedCount = 0;

        foreach (var entity in entities)
        {
            if (!await query.AnyAsync(
                    b => b.CountryId == entity.CountryId && string.Equals(b.Name, entity.Name,
                        StringComparison.InvariantCultureIgnoreCase), cancellationToken))
            {
                await Entities.AddAsync(entity, cancellationToken);
                addedCount++;
                logger.Debug("Добавлен банк: {BankName} (страна: {CountryId})", entity.Name, entity.CountryId);
            }
            else
            {
                logger.Debug("Банк {BankName} уже существует в стране {CountryId}, пропускаем", entity.Name,
                    entity.CountryId);
            }
        }

        var commitResult = await _context.CommitAsync(cancellationToken);

        logger.Information("Инициализация завершена, добавлено {AddedCount} новых банков", addedCount);

        return commitResult;
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
        logger.Information("Получение всех банков. Включить связанные данные: {IncludeRelated}", includeRelated);

        var query = Entities.AsNoTracking();
        if (includeRelated)
            query = IncludeRelatedEntities(query);
        var banks = await query.ToListAsync(cancellationToken);

        logger.Information("Получено {BanksCount} банков", banks.Count);

        return banks;
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
        logger.Debug("Проверка уникальности имени банка '{BankName}' в стране {CountryId}, исключая банк {ExcludeId}",
            name, countryId, excludeId);

        var query = Entities.Where(b => b.CountryId == countryId);
        var isUnique = await IsUniqueAsync(query,
            predicate: b => string.Equals(b.Name, name, StringComparison.InvariantCultureIgnoreCase),
            excludeId, cancellationToken);

        logger.Debug("Имя банка '{BankName}' в стране {CountryId} {UniqueResult}",
            name, countryId, isUnique ? "уникально" : "не уникально");

        return isUnique;
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
        logger.Information("Получение количества счетов для банка {BankId}. " +
                           "Включить архивные: {IncludeArchived}, " +
                           "Включить удалённые: {IncludeDeleted}",
            bankId, includeArchivedAccounts, includeDeletedAccounts);

        var query = _context.Accounts.Where(b => b.BankId == bankId);
        if (!includeArchivedAccounts)
            query = query.Where(a => a.IsArchived == false);
        if (!includeDeletedAccounts)
            query = query.Where(a => a.IsDeleted == false);

        var count = await query.CountAsync(cancellationToken);

        logger.Information("Банк {BankId} используется в {AccountsCount} счетах", bankId, count);

        return count;
    }

    /// <summary>
    /// Определяет, может ли банк быть удалён (т.е. не используется ни одним счётом).
    /// </summary>
    /// <param name="bankId">Идентификатор банка для проверки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если банк можно удалить; иначе — false.</returns>
    public async Task<bool> CanBeDeletedAsync(Guid bankId, CancellationToken cancellationToken = default)
    {
        logger.Debug("Проверка возможности удаления банка {BankId}", bankId);

        var canBeDeleted = !await _context.Accounts.AnyAsync(a => a.BankId == bankId, cancellationToken);

        logger.Information("Банк {BankId} {DeletionResult}", bankId,
            canBeDeleted ? "может быть удалён" : "не может быть удалён (используется в счетах)");

        return canBeDeleted;
    }
}