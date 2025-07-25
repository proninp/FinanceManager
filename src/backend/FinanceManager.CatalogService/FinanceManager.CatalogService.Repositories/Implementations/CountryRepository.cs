using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.Countries;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FinanceManager.CatalogService.Repositories.Implementations;

/// <summary>
/// Репозиторий для управления сущностями <see cref="Country"/>.
/// Предоставляет методы для фильтрации, инициализации, проверки уникальности и сортировки.
/// </summary>
public class CountryRepository(DatabaseContext context, ILogger logger)
    : BaseRepository<Country, CountryFilterDto>(context, logger), ICountryRepository
{
    private readonly DatabaseContext _context = context;

    /// <summary>
    /// Применяет фильтры к запросу <see cref="Country"/> на основе переданного <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">DTO фильтра с критериями фильтрации.</param>
    /// <param name="query">Исходный запрос для применения фильтров.</param>
    /// <returns>Отфильтрованный <see cref="IQueryable{Country}"/>.</returns>
    private protected override IQueryable<Country> SetFilters(CountryFilterDto filter, IQueryable<Country> query)
    {
        if (filter.NameContains != null)
        {
            query = filter.NameContains.Length > 0
                ? query.Where(c => c.Name.Contains(filter.NameContains))
                : query.Where(c => string.Equals(c.Name, string.Empty));
        }

        return query;
    }

    /// <summary>
    /// Инициализирует репозиторий набором сущностей <see cref="Country"/>, если они ещё не существуют.
    /// Добавляет только уникальные страны по имени.
    /// </summary>
    /// <param name="entities">Коллекция стран для инициализации.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Количество записей, сохранённых в базе данных.</returns>
    public async Task<int> InitializeAsync(IEnumerable<Country> entities, CancellationToken cancellationToken = default)
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
                        StringComparison.InvariantCultureIgnoreCase), cancellationToken))
            {
                await Entities.AddAsync(entity, cancellationToken);
            }
        }
        return await _context.CommitAsync(cancellationToken);
    }

    /// <summary>
    /// Получает все сущности <see cref="Country"/>, отсортированные по имени.
    /// </summary>
    /// <param name="ascending">Если true — сортировка по возрастанию, иначе — по убыванию.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Коллекция стран, отсортированных по имени.</returns>
    public async Task<ICollection<Country>> GetAllOrderedByNameAsync(bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = Entities.AsNoTracking();
        if (ascending)
            return await query.OrderBy(c => c.Name).ToListAsync(cancellationToken);
        return await query.OrderByDescending(c => c.Name).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Проверяет уникальность имени страны в репозитории.
    /// </summary>
    /// <param name="name">Имя страны для проверки.</param>
    /// <param name="excludeId">Необязательный идентификатор страны, которую нужно исключить из проверки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если имя уникально; иначе — false.</returns>
    public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await IsUniqueAsync(Entities.AsQueryable(),
            predicate: c => string.Equals(c.Name, name, StringComparison.InvariantCultureIgnoreCase),
            excludeId, cancellationToken);
    }

    /// <summary>
    /// Определяет, может ли страна быть удалена (т.е. не используется ни одним банком).
    /// </summary>
    /// <param name="countryId">Идентификатор страны для проверки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если страну можно удалить; иначе — false.</returns>
    public async Task<bool> CanBeDeletedAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        return !await _context.Banks
            .AnyAsync(b => b.CountryId == countryId, cancellationToken);
    }
}