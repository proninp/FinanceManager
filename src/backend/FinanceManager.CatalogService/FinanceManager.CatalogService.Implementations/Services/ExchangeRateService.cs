using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.ExchangeRates;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Services;

/// <summary>
/// Сервис для управления обменными курсами валют
/// Предоставляет методы для получения, создания, обновления и удаления обменных курсов
/// </summary>
public class ExchangeRateService(
    IUnitOfWork unitOfWork,
    IExchangeRateRepository exchangeRateRepository,
    IExchangeRateErrorsFactory exchangeRateErrorsFactory,
    ILogger logger) : IExchangeRateService
{
    /// <summary>
    /// Получает обменный курс по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор курса</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с DTO курса или ошибкой, если не найден</returns>
    public async Task<Result<ExchangeRateDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Getting exchange rate by id: {ExchangeRateId}", id);

        var rate = await exchangeRateRepository.GetByIdAsync(id, disableTracking: true,
            cancellationToken: cancellationToken);
        if (rate is null)
        {
            return Result.Fail(exchangeRateErrorsFactory.NotFound(id));
        }

        logger.Information("Successfully retrieved exchange rate: {ExchangeRateId}", id);
        return Result.Ok(rate.ToDto());
    }

    /// <summary>
    /// Получает список обменных курсов с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком курсов или ошибкой</returns>
    public async Task<Result<ICollection<ExchangeRateDto>>> GetPagedAsync(ExchangeRateFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Getting paged exchange rates with filter: {@Filter}", filter);
        var rates = await exchangeRateRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);

        var ratesDto = rates.ToDto();

        logger.Information("Successfully retrieved {Count} exchange rates", ratesDto.Count);
        return Result.Ok(ratesDto);
    }

    /// <summary>
    /// Создаёт новый обменный курс
    /// </summary>
    /// <param name="createDto">Данные для создания курса</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданным курсом или ошибкой</returns>
    public async Task<Result<ExchangeRateDto>> CreateAsync(CreateExchangeRateDto createDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Creating exchange rate: {@CreateDto}", createDto);
        if (createDto.CurrencyId == Guid.Empty)
        {
            return Result.Fail(exchangeRateErrorsFactory.CurrencyIsRequired());
        }

        if (createDto.RateDate == default)
        {
            return Result.Fail(exchangeRateErrorsFactory.RateDateIsRequired());
        }

        if (createDto.Rate == 0)
        {
            return Result.Fail(exchangeRateErrorsFactory.RateValueIsRequired());
        }

        if (await exchangeRateRepository.ExistsForCurrencyAndDateAsync(createDto.CurrencyId, createDto.RateDate,
                cancellationToken))
        {
            return Result.Fail(exchangeRateErrorsFactory.AlreadyExists(createDto.CurrencyId, createDto.RateDate));
        }

        var rate = await exchangeRateRepository.AddAsync(createDto.ToExchangeRate(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Successfully created exchange rate: {ExchangeRateId}", rate.Id);

        return Result.Ok(rate.ToDto());
    }

    /// <summary>
    /// Добавляет несколько курсов валют за один раз
    /// </summary>
    /// <param name="createExchangeRatesDto">Список курсов для добавления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с количеством добавленных курсов или ошибкой</returns>
    public async Task<Result<int>> AddRangeAsync(IEnumerable<CreateExchangeRateDto> createExchangeRatesDto,
        CancellationToken cancellationToken = default)
    {
        var createDtoList = createExchangeRatesDto as IList<CreateExchangeRateDto> ?? createExchangeRatesDto.ToList();
        logger.Information("Adding range of exchange rates: {@CreateExchangeRatesDto}", createDtoList);

        var entities = createDtoList.Select(x => x.ToExchangeRate()).ToList();
        await exchangeRateRepository.AddRangeAsync(entities, cancellationToken);
        var result = await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Successfully added {Count} exchange rates", result);
        return Result.Ok(result);
    }

    /// <summary>
    /// Обновляет существующий обменный курс
    /// </summary>
    /// <param name="updateDto">Данные для обновления курса</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленным курсом или ошибкой</returns>
    public async Task<Result<ExchangeRateDto>> UpdateAsync(UpdateExchangeRateDto updateDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Updating exchange rate: {@UpdateDto}", updateDto);

        var rate = await exchangeRateRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (rate is null)
        {
            return Result.Fail(exchangeRateErrorsFactory.NotFound(updateDto.Id));
        }

        var isNeedUpdate = false;

        if (updateDto.RateDate.HasValue && rate.RateDate != updateDto.RateDate.Value)
        {
            if (await exchangeRateRepository.ExistsForCurrencyAndDateAsync(rate.CurrencyId, updateDto.RateDate.Value,
                    cancellationToken))
            {
                return Result.Fail(exchangeRateErrorsFactory.AlreadyExists(rate.CurrencyId, updateDto.RateDate.Value));
            }

            rate.RateDate = updateDto.RateDate.Value;
            isNeedUpdate = true;
        }

        if (updateDto.Rate.HasValue && rate.Rate != updateDto.Rate.Value)
        {
            rate.Rate = updateDto.Rate.Value;
            isNeedUpdate = true;
        }

        if (isNeedUpdate)
        {
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Successfully updated exchange rate: {ExchangeRateId}", updateDto.Id);
        }
        else
        {
            logger.Information("No changes detected for exchange rate: {ExchangeRateId}", updateDto.Id);
        }

        return Result.Ok(rate.ToDto());
    }

    /// <summary>
    /// Удаляет обменный курс по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор курса</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат выполнения операции</returns>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Deleting exchange rate: {ExchangeRateId}", id);

        await exchangeRateRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows == 0)
        {
            logger.Warning("No exchange rate was deleted for id: {ExchangeRateId}", id);
        }

        return Result.Ok();
    }

    /// <summary>
    /// Проверяет существование курса для валюты на определенную дату
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="rateDate">Дата курса</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки существования</returns>
    public async Task<Result<bool>> ExistsForCurrencyAndDateAsync(Guid currencyId, DateTime rateDate,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Checking if exchange rate exists for currency {CurrencyId} on {RateDate}", currencyId,
            rateDate);

        var exists =
            await exchangeRateRepository.ExistsForCurrencyAndDateAsync(currencyId, rateDate, cancellationToken);
        return Result.Ok(exists);
    }

    /// <summary>
    /// Получает дату последнего курса валюты
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с датой последнего курса или ошибкой</returns>
    public async Task<Result<DateTime?>> GetLastRateDateAsync(Guid currencyId,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Getting last rate date for currency: {CurrencyId}", currencyId);

        var date = await exchangeRateRepository.GetLastRateDateAsync(currencyId, cancellationToken);
        return Result.Ok(date);
    }

    /// <summary>
    /// Удаляет курсы валюты за определенный период
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="dateFrom">Дата начала периода</param>
    /// <param name="dateTo">Дата окончания периода</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с количеством удаленных курсов или ошибкой</returns>
    public async Task<Result<int>> DeleteByPeriodAsync(Guid currencyId, DateTime dateFrom, DateTime dateTo,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Deleting exchange rates for currency {CurrencyId} from {DateFrom} to {DateTo}", currencyId,
            dateFrom, dateTo);

        await exchangeRateRepository.DeleteByPeriodAsync(currencyId, dateFrom, dateTo, cancellationToken);
        var deletedCount = await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Deleted {Count} exchange rates for currency {CurrencyId}", deletedCount, currencyId);
        return Result.Ok(deletedCount);
    }
}