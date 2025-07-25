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
        if (!await Entities.AnyAsync(cancellationToken))
        {
            await Entities.AddRangeAsync(entities, cancellationToken);
            return await _context.CommitAsync(cancellationToken);
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
        return await _context.CommitAsync(cancellationToken);
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
        return await GetAllOrderedByAsync(c => c.Name, includeDeleted, ascending, cancellationToken);
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
        return await GetAllOrderedByAsync(c => c.CharCode, includeDeleted, ascending, cancellationToken);
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
        var query = Entities.AsQueryable();
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        return !await query.AnyAsync(
            c => string.Equals(c.CharCode, charCode, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken: cancellationToken);
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
        var query = Entities.AsQueryable();
        return await IsUniqueAsync(query,
            predicate: c => string.Equals(c.NumCode, numCode, StringComparison.InvariantCultureIgnoreCase),
            excludeId, cancellationToken);
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
        var query = Entities.AsQueryable();
        return await IsUniqueAsync(query,
            predicate: c => string.Equals(c.Name, name, StringComparison.InvariantCultureIgnoreCase),
            excludeId, cancellationToken);
    }

    /// <summary>
    /// Проверяет возможность удаления валюты.
    /// </summary>
    /// <param name="id">Идентификатор валюты.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если валюта может быть удалена, иначе False.</returns>
    public async Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await _context.ExchageRates.AnyAsync(er => er.CurrencyId == id, cancellationToken))
            return false;
        return !await _context.Accounts.AnyAsync(a => a.CurrencyId == id, cancellationToken);
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
        var query = Entities.Where(c => c.Id == id);
        if (!includeDeleted)
            query = query.Where(a => !a.IsDeleted);
        return await query.AnyAsync(cancellationToken);
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