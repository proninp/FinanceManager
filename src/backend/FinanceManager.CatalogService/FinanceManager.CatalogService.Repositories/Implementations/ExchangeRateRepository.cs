using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.ExchangeRates;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.CatalogService.Repositories.Implementations;

public class ExchangeRateRepository(DatabaseContext context)
    : BaseRepository<ExchangeRate, ExchangeRateFilterDto>(context), IExchangeRateRepository
{
    private readonly DatabaseContext _context = context;

    private protected override IQueryable<ExchangeRate> IncludeRelatedEntities(IQueryable<ExchangeRate> query)
    {
        return query
            .Include(er => er.Currency);
    }

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

    public async Task<bool> ExistsForCurrencyAndDateAsync(Guid currencyId, DateTime rateDate,
        CancellationToken cancellationToken = default)
    {
        return await Entities.AnyAsync(er => er.CurrencyId == currencyId && er.RateDate == rateDate,
            cancellationToken: cancellationToken);
    }
    
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

    public async Task<bool> ExistsForDateAsync(DateTime rateDate, CancellationToken cancellationToken = default)
    {
        return await Entities.AnyAsync(er => er.RateDate == rateDate,
            cancellationToken: cancellationToken);
    }

    public async Task<DateTime?> GetLastRateDateAsync(Guid currencyId, CancellationToken cancellationToken = default)
    {
        return await Entities
            .Where(er => er.CurrencyId == currencyId)
            .Select(er => (DateTime?)er.RateDate)
            .MaxAsync(cancellationToken);
    }
    
    public async Task DeleteByPeriodAsync(Guid currencyId, DateTime dateFrom, DateTime dateTo,
        CancellationToken cancellationToken = default)
    {
        await Entities.Where(er => er.CurrencyId == currencyId && er.RateDate >= dateFrom && er.RateDate <= dateTo)
            .ExecuteDeleteAsync(cancellationToken);
    }
}