using System.Linq.Expressions;
using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.Currencies;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FinanceManager.CatalogService.Repositories.Implementations;

/// <summary>
/// Репозиторий для работы с валютами.
/// Предоставляет методы для управления валютами, включая фильтрацию, инициализацию, проверку уникальности и сортировку.
/// </summary>
/// <remarks>
/// Наследует функциональность базового репозитория и реализует ICurrencyRepository.
/// </remarks>
public class CurrencyRepository(DatabaseContext context, ILogger logger)
    : BaseRepository<Currency, CurrencyFilterDto>(context, logger), ICurrencyRepository
{
    private readonly DatabaseContext _context = context;

    /// <summary>
    /// Применяет фильтры к запросу валют.
    /// </summary>
    /// <param name="filter">Параметры фильтрации.</param>
    /// <param name="query">Исходный запрос.</param>
    /// <returns>Отфильтрованный запрос.</returns>
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

    /// <summary>
    /// Инициализирует репозиторий набором валют, если он пуст.
    /// </summary>
    /// <param name="entities">Коллекция валют для инициализации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Количество добавленных записей.</returns>
    public async Task<int> InitializeAsync(IEnumerable<Currency> entities,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Начинается инициализация валют");

        var currenciesList = entities as ICollection<Currency> ?? entities.ToList();
        logger.Debug("Подготовлено {CurrenciesCount} валют для инициализации", currenciesList.Count);

        if (!await Entities.AnyAsync(cancellationToken))
        {
            logger.Debug("Таблица валют пуста, добавляем все валюты");
            await Entities.AddRangeAsync(currenciesList, cancellationToken);
            var result = await _context.CommitAsync(cancellationToken);
            logger.Information("Инициализация завершена, добавлено {AddedCount} валют", result);
            return result;
        }

        logger.Debug("Таблица валют содержит данные, проверяем уникальность");
        var query = Entities.AsQueryable();
        var addedCount = 0;

        foreach (var entity in currenciesList)
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
                addedCount++;
                logger.Debug("Добавлена валюта: {CurrencyName} ({CharCode})",
                    entity.Name, entity.CharCode);
            }
            else
            {
                logger.Debug("Валюта {CurrencyName} ({CharCode}) уже существует, пропускаем",
                    entity.Name, entity.CharCode);
            }
        }

        var commitResult = await _context.CommitAsync(cancellationToken);
        logger.Information("Инициализация завершена, добавлено {AddedCount} новых валют из {TotalCount}",
            addedCount, currenciesList.Count);

        return commitResult;
    }

    /// <summary>
    /// Получает все валюты, отсортированные по названию.
    /// </summary>
    /// <param name="includeDeleted">Включать удаленные валюты.</param>
    /// <param name="ascending">Сортировка по возрастанию.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция валют.</returns>
    public async Task<ICollection<Currency>> GetAllOrderedByNameAsync(bool includeDeleted = false,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        logger.Information(
            "Получение всех валют, отсортированных по названию. " +
            "Включить удалённые: {IncludeDeleted}, По возрастанию: {Ascending}",
            includeDeleted, ascending);

        var currencies = await GetAllOrderedByAsync(
            c => c.Name, includeDeleted, ascending, cancellationToken);
        logger.Information("Получено {CurrenciesCount} валют, отсортированных по названию", currencies.Count);

        return currencies;
    }

    /// <summary>
    /// Получает все валюты, отсортированные по буквенному коду.
    /// </summary>
    /// <param name="includeDeleted">Включать удаленные валюты.</param>
    /// <param name="ascending">Сортировка по возрастанию.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция валют.</returns>
    public async Task<ICollection<Currency>> GetAllOrderedByCharCodeAsync(bool includeDeleted = false,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        logger.Information(
            "Получение всех валют, отсортированных по символьному коду. " +
            "Включить удалённые: {IncludeDeleted}, По возрастанию: {Ascending}",
            includeDeleted, ascending);

        var currencies = await GetAllOrderedByAsync(c => c.CharCode, includeDeleted, ascending, cancellationToken);

        logger.Information("Получено {CurrenciesCount} валют, отсортированных по символьному коду", currencies.Count);

        return currencies;
    }

    /// <summary>
    /// Проверяет уникальность буквенного кода валюты.
    /// </summary>
    /// <param name="charCode">Буквенный код для проверки.</param>
    /// <param name="excludeId">Идентификатор валюты, которую следует исключить из проверки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если код уникален, иначе False.</returns>
    public async Task<bool> IsCharCodeUniqueAsync(string charCode, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        logger.Debug("Проверка уникальности символьного кода валюты '{CharCode}', исключая валюту {ExcludeId}", 
            charCode, excludeId);
        var query = Entities.AsQueryable();
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        var isUnique = !await query.AnyAsync(
            c => string.Equals(c.CharCode, charCode, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken: cancellationToken);

        logger.Debug("Символьный код валюты '{CharCode}' {UniqueResult}", 
            charCode, isUnique ? "уникален" : "не уникален");
        
        return isUnique;
    }

    /// <summary>
    /// Проверяет уникальность цифрового кода валюты.
    /// </summary>
    /// <param name="numCode">Цифровой код для проверки.</param>
    /// <param name="excludeId">Идентификатор валюты, которую следует исключить из проверки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если код уникален, иначе False.</returns>
    public async Task<bool> IsNumCodeUniqueAsync(string numCode, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        logger.Debug("Проверка уникальности числового кода валюты '{NumCode}', исключая валюту {ExcludeId}", 
            numCode, excludeId);
        var query = Entities.AsQueryable();
        var isUnique = await IsUniqueAsync(query,
            predicate: c => string.Equals(c.NumCode, numCode, StringComparison.InvariantCultureIgnoreCase),
            excludeId, cancellationToken);

        logger.Debug("Числовой код валюты '{NumCode}' {UniqueResult}", 
            numCode, isUnique ? "уникален" : "не уникален");
        
        return isUnique;
    }

    /// <summary>
    /// Проверяет уникальность названия валюты.
    /// </summary>
    /// <param name="name">Название для проверки.</param>
    /// <param name="excludeId">Идентификатор валюты, которую следует исключить из проверки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если название уникально, иначе False.</returns>
    public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        logger.Debug("Проверка уникальности названия валюты '{Name}', исключая валюту {ExcludeId}", 
            name, excludeId);
        var query = Entities.AsQueryable();
        var isUnique =  await IsUniqueAsync(query,
            predicate: c => string.Equals(c.Name, name, StringComparison.InvariantCultureIgnoreCase),
            excludeId, cancellationToken);
        
        logger.Debug("Название валюты '{Name}' {UniqueResult}", 
            name, isUnique ? "уникально" : "не уникально");

        return isUnique;
    }

    /// <summary>
    /// Проверяет возможность удаления валюты.
    /// </summary>
    /// <param name="id">Идентификатор валюты.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если валюта может быть удалена, иначе False.</returns>
    public async Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Проверка возможности удаления валюты {CurrencyId}", id);
        if (await _context.ExchageRates.AnyAsync(er => er.CurrencyId == id, cancellationToken))
        {
            logger.Information("Валюта {CurrencyId} не может быть удалена (используется в курсах валют)", id);
            return false;
        }
        var hasAccounts = await _context.Accounts.AnyAsync(a => a.CurrencyId == id, cancellationToken);
        var canBeDeleted = !hasAccounts;

        logger.Information("Валюта {CurrencyId} {DeletionResult}", 
            id, canBeDeleted ? "может быть удалена" : "не может быть удалена (используется в счетах)");
        
        return canBeDeleted;
    }

    /// <summary>
    /// Проверяет существование валюты с указанным идентификатором.
    /// </summary>
    /// <param name="id">Идентификатор валюты.</param>
    /// <param name="includeDeleted">Включать удаленные валюты.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если валюта существует, иначе False.</returns>
    public async Task<bool> ExistsAsync(Guid id, bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        logger.Debug("Проверка существования валюты {CurrencyId}. Включить удалённые: {IncludeDeleted}", 
            id, includeDeleted);
        
        var query = Entities.Where(c => c.Id == id);
        if (!includeDeleted)
            query = query.Where(a => !a.IsDeleted);
        var exists = await query.AnyAsync(cancellationToken);

        logger.Debug("Валюта {CurrencyId} {ExistsResult}", id, exists ? "существует" : "не существует");
        
        return exists;
    }

    /// <summary>
    /// Получает все валюты, отсортированные по указанному полю.
    /// </summary>
    /// <param name="selector">Выражение для сортировки.</param>
    /// <param name="includeDeleted">Включать удаленные валюты.</param>
    /// <param name="ascending">Сортировка по возрастанию.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция отсортированных валют.</returns>
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