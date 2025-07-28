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
    private readonly ILogger _logger = logger;

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
        _logger.Information("Начинается инициализация стран");
        var countriesList = entities as ICollection<Country> ?? entities.ToList();
        _logger.Debug("Подготовлено {CountriesCount} стран для инициализации", countriesList.Count);
        
        if (!await Entities.AnyAsync(cancellationToken))
        {
            _logger.Debug("Таблица стран пуста, добавляем все страны");
            await Entities.AddRangeAsync(countriesList, cancellationToken);
            var result = await _context.CommitAsync(cancellationToken);
                
            _logger.Information("Инициализация завершена, добавлено {AddedCount} стран", result);
            return result;
        }

        _logger.Debug("Таблица стран содержит данные, проверяем уникальность");
        var query = Entities.AsQueryable();
        var addedCount = 0;
        
        foreach (var entity in countriesList)
        {
            if (!await query.AnyAsync(
                    c => string.Equals(c.Name, entity.Name,
                        StringComparison.InvariantCultureIgnoreCase), cancellationToken))
            {
                await Entities.AddAsync(entity, cancellationToken);
                addedCount++;
                _logger.Debug("Добавлена страна: {CountryName}", entity.Name);
            }
            else
            {
                _logger.Debug("Валюта {CountryName} уже существует, пропускаем", entity.Name);
            }
        }
        var commitResult = await _context.CommitAsync(cancellationToken);
            
        _logger.Information("Инициализация завершена, добавлено {AddedCount} новых валют из {TotalCount}",
            addedCount, countriesList.Count);
            
        return commitResult;
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
        _logger.Information(
            "Получение всех стран, отсортированных по названию. " +
            "По возрастанию: {Ascending}", ascending);
        var query = Entities.AsNoTracking();
        var countries = ascending
            ? await query.OrderBy(c => c.Name).ToListAsync(cancellationToken)
            : await query.OrderByDescending(c => c.Name).ToListAsync(cancellationToken);

        _logger.Information("Получено {Count} стран, отсортированных по имени ({Order})", 
            countries.Count, ascending ? "по возрастанию" : "по убыванию");
        
        return countries;
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
        _logger.Debug("Проверка уникальности названия страны '{CountryName}', исключая страну {ExcludeId}", 
            name, excludeId);
        var isUnique = await IsUniqueAsync(Entities.AsQueryable(),
            predicate: c => string.Equals(c.Name, name, StringComparison.InvariantCultureIgnoreCase),
            excludeId, cancellationToken);

        _logger.Information("Проверка уникальности имени страны: '{Name}' является {IsUnique}", 
            name, isUnique ? "уникальным" : "неуникальным");
        
        return isUnique;
    }

    /// <summary>
    /// Определяет, может ли страна быть удалена (т.е. не используется ни одним банком).
    /// </summary>
    /// <param name="id">Идентификатор страны для проверки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>True, если страну можно удалить; иначе — false.</returns>
    public async Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.Information("Проверка возможности удаления страны {CountryId}", id);
        var hasBanks = await _context.Banks
            .AnyAsync(b => b.CountryId == id, cancellationToken);
        var canBeDeleted = !hasBanks;
        
        _logger.Information("Страна {CountryId} {DeletionResult}", 
            id, canBeDeleted ? "может быть удалена" : "не может быть удалена (используется в банках)");
        
        return canBeDeleted;
    }
}