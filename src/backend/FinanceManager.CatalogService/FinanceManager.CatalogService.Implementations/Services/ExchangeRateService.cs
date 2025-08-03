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
        logger.Information("Получение курса валют по идентификатору: {ExchangeRateId}", id);

        var rate = await exchangeRateRepository.GetByIdAsync(id, disableTracking: true,
            cancellationToken: cancellationToken);
        if (rate is null)
        {
            logger.Warning("Курс валют с идентификатором {ExchangeRateId} не найден", id);
            return Result.Fail(exchangeRateErrorsFactory.NotFound(id));
        }

        logger.Information("Курс валют {ExchangeRateId} успешно получен", id);
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
        logger.Information("Получение списка курсов валют с фильтрацией: {@Filter}", filter);
        
        var rates = await exchangeRateRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);

        var ratesDto = rates.ToDto();

        logger.Information("Получено {Count} курсов валют", ratesDto.Count);
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
        logger.Information("Создание нового курса валют: {@CreateDto}", createDto);
        
        if (createDto.CurrencyId == Guid.Empty)
        {
            logger.Warning("Попытка создания курса валют без указания валюты");
            return Result.Fail(exchangeRateErrorsFactory.CurrencyIsRequired());
        }

        if (createDto.RateDate == default)
        {
            logger.Warning("Попытка создания курса валют без указания даты");
            return Result.Fail(exchangeRateErrorsFactory.RateDateIsRequired());
        }

        if (createDto.Rate == 0)
        {
            logger.Warning("Попытка создания курса валют без указания значения курса");
            return Result.Fail(exchangeRateErrorsFactory.RateValueIsRequired());
        }

        if (await exchangeRateRepository.ExistsForCurrencyAndDateAsync(createDto.CurrencyId, createDto.RateDate,
                cancellationToken))
        {
            logger.Warning("Попытка создания курса валют для валюты {CurrencyId} на дату {RateDate}, который уже существует", 
                createDto.CurrencyId, createDto.RateDate);
            return Result.Fail(exchangeRateErrorsFactory.AlreadyExists(createDto.CurrencyId, createDto.RateDate));
        }

        var rate = await exchangeRateRepository.AddAsync(createDto.ToExchangeRate(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Курс валют {ExchangeRateId} для валюты {CurrencyId} на дату {RateDate} со значением {Rate} успешно создан",
            rate.Id, createDto.CurrencyId, createDto.RateDate, createDto.Rate);

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
        logger.Information("Добавление списка курсов валют: количество элементов {Count}", createDtoList.Count);

        var entities = createDtoList.Select(x => x.ToExchangeRate()).ToList();
        await exchangeRateRepository.AddRangeAsync(entities, cancellationToken);
        var result = await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Успешно добавлено {Count} курсов валют", result);
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
        logger.Information("Обновление курса валют: {@UpdateDto}", updateDto);

        var rate = await exchangeRateRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (rate is null)
        {
            logger.Warning("Курс валют с идентификатором {ExchangeRateId} не найден для обновления", updateDto.Id);
            return Result.Fail(exchangeRateErrorsFactory.NotFound(updateDto.Id));
        }

        var isNeedUpdate = false;

        if (updateDto.RateDate.HasValue && rate.RateDate != updateDto.RateDate.Value)
        {
            if (await exchangeRateRepository.ExistsForCurrencyAndDateAsync(rate.CurrencyId, updateDto.RateDate.Value,
                    cancellationToken))
            {
                logger.Warning("Попытка обновления курса валют {ExchangeRateId} на дату {RateDate}, для которой уже существует курс валюты {CurrencyId}", 
                    updateDto.Id, updateDto.RateDate.Value, rate.CurrencyId);
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
            logger.Information("Курс валют {ExchangeRateId} успешно обновлен", updateDto.Id);
        }
        else
        {
            logger.Information("Изменения для курса валют {ExchangeRateId} не обнаружены", updateDto.Id);
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
        logger.Information("Удаление курса валют: {ExchangeRateId}", id);

        await exchangeRateRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows > 0)
        {
            logger.Information("Курс валют {ExchangeRateId} успешно удален", id);
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
        logger.Debug("Проверка существования курса для валюты {CurrencyId} на дату {RateDate}", currencyId, rateDate);

        var exists =
            await exchangeRateRepository.ExistsForCurrencyAndDateAsync(currencyId, rateDate, cancellationToken);
            
        logger.Debug("Курс для валюты {CurrencyId} на дату {RateDate} {ExistsResult}", 
            currencyId, rateDate, exists ? "существует" : "не существует");
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
        logger.Information("Получение даты последнего курса для валюты: {CurrencyId}", currencyId);

        var date = await exchangeRateRepository.GetLastRateDateAsync(currencyId, cancellationToken);
        
        if (date.HasValue)
        {
            logger.Information("Для валюты {CurrencyId} найдена дата последнего курса: {LastRateDate}", 
                currencyId, date.Value);
        }
        else
        {
            logger.Information("Для валюты {CurrencyId} не найдено ни одного курса", currencyId);
        }
        
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
        logger.Information("Удаление курсов валют для валюты {CurrencyId} за период с {DateFrom:yyyy-MM-dd} по {DateTo:yyyy-MM-dd}", 
            currencyId, dateFrom, dateTo);

        await exchangeRateRepository.DeleteByPeriodAsync(currencyId, dateFrom, dateTo, cancellationToken);
        var deletedCount = await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Для валюты {CurrencyId} удалено {Count} курсов за период с {DateFrom:yyyy-MM-dd} по {DateTo:yyyy-MM-dd}", 
            currencyId, deletedCount, dateFrom, dateTo);
        return Result.Ok(deletedCount);
    }
}