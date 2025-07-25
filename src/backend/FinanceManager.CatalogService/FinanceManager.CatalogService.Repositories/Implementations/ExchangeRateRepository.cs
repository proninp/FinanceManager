using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.ExchangeRates;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FinanceManager.CatalogService.Repositories.Implementations;

/// <summary>
/// Репозиторий для работы с курсами валют.
/// Предоставляет методы для управления курсами валют, включая фильтрацию, добавление, проверку существования и удаление.
/// </summary>
/// <remarks>
/// Наследует функциональность базового репозитория и реализует IExchangeRateRepository.
/// </remarks>
public class ExchangeRateRepository(DatabaseContext context, ILogger logger)
    : BaseRepository<ExchangeRate, ExchangeRateFilterDto>(context, logger), IExchangeRateRepository
{
    private readonly DatabaseContext _context = context;

    /// <summary>
    /// Включает связанные сущности (валюту) в запрос.
    /// </summary>
    /// <param name="query">Исходный запрос.</param>
    /// <returns>Запрос с включенными связанными сущностями.</returns>
    private protected override IQueryable<ExchangeRate> IncludeRelatedEntities(IQueryable<ExchangeRate> query)
    {
        return query
            .Include(er => er.Currency);
    }

    /// <summary>
    /// Применяет фильтры к запросу курсов валют.
    /// </summary>
    /// <param name="filter">Параметры фильтрации.</param>
    /// <param name="query">Исходный запрос.</param>
    /// <returns>Отфильтрованный запрос.</returns>
    private protected override IQueryable<ExchangeRate> SetFilters(ExchangeRateFilterDto filter,
        IQueryable<ExchangeRate> query)
    {
        if (filter.CurrencyId.HasValue)
            query = query.Where(er => er.CurrencyId == filter.CurrencyId.Value);
        if (filter.DateFrom.HasValue)
            query = query.Where(er => er.RateDate >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue)
            query = query.Where(er => er.RateDate <= filter.DateTo.Value);
        if (filter.RateFrom.HasValue)
            query = query.Where(er => er.Rate >= filter.RateFrom.Value);
        if (filter.RateTo.HasValue)
            query = query.Where(er => er.Rate <= filter.RateTo.Value);
        return query;
    }

    /// <summary>
    /// Проверяет существование курса валюты для указанной валюты и даты.
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты.</param>
    /// <param name="rateDate">Дата курса.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если курс существует, иначе False.</returns>
    public async Task<bool> ExistsForCurrencyAndDateAsync(Guid currencyId, DateTime rateDate,
        CancellationToken cancellationToken = default)
    {
        return await Entities.AnyAsync(er => er.CurrencyId == currencyId && er.RateDate == rateDate,
            cancellationToken: cancellationToken);
    }
    
    /// <summary>
    /// Добавляет коллекцию курсов валют, пропуская уже существующие.
    /// </summary>
    /// <param name="exchangeRates">Коллекция курсов для добавления.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция фактически добавленных курсов.</returns>
    public async Task<ICollection<ExchangeRate>> AddRangeAsync(ICollection<ExchangeRate> exchangeRates,
        CancellationToken cancellationToken = default)
    {
        if (!await Entities.AnyAsync(cancellationToken))
        {
            await Entities.AddRangeAsync(exchangeRates, cancellationToken);
            return exchangeRates;
        }

        var addedRates = new List<ExchangeRate>();
        var query = Entities.AsQueryable();
        foreach (var entity in exchangeRates)
        {
            if (await query.AnyAsync(
                    er => er.CurrencyId == entity.CurrencyId && er.RateDate == entity.RateDate,
                    cancellationToken)) continue;
            await Entities.AddAsync(entity, cancellationToken);
            addedRates.Add(entity);
        }
        return addedRates;
    }

    /// <summary>
    /// Проверяет существование хотя бы одного курса валюты для указанной даты.
    /// </summary>
    /// <param name="rateDate">Дата для проверки.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если курс существует, иначе False.</returns>
    public async Task<bool> ExistsForDateAsync(DateTime rateDate, CancellationToken cancellationToken = default)
    {
        return await Entities.AnyAsync(er => er.RateDate == rateDate,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Возвращает последнюю дату курса для указанной валюты.
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Дата последнего курса или null, если записи отсутствуют.</returns>
    public async Task<DateTime?> GetLastRateDateAsync(Guid currencyId, CancellationToken cancellationToken = default)
    {
        return await Entities
            .Where(er => er.CurrencyId == currencyId)
            .Select(er => (DateTime?)er.RateDate)
            .MaxAsync(cancellationToken);
    }
    
    /// <summary>
    /// Удаляет курсы валют за указанный период для заданной валюты.
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты.</param>
    /// <param name="dateFrom">Начало периода.</param>
    /// <param name="dateTo">Конец периода.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public async Task DeleteByPeriodAsync(Guid currencyId, DateTime dateFrom, DateTime dateTo,
        CancellationToken cancellationToken = default)
    {
        await Entities.Where(er => er.CurrencyId == currencyId && er.RateDate >= dateFrom && er.RateDate <= dateTo)
            .ExecuteDeleteAsync(cancellationToken);
    }
}