using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.Currencies;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Services;

public class CurrencyService(
    IUnitOfWork unitOfWork,
    ICurrencyRepository currencyRepository,
    ICurrencyErrorsFactory currencyErrorsFactory,
    ILogger logger) : ICurrencyService
{
    /// <summary>
    /// Получает валюту по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с DTO валюты или ошибкой, если не найдена</returns>
    public async Task<Result<CurrencyDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Getting currency by id: {CurrencyId}", id);

        var currency =
            await currencyRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (currency is null)
        {
            return Result.Fail(currencyErrorsFactory.NotFound(id));
        }

        logger.Information("Successfully retrieved currency: {CurrencyId}", id);
        return Result.Ok(currency.ToDto());
    }

    /// <summary>
    /// Получает список валют с пагинацией и фильтрацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации и пагинации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с коллекцией DTO валют</returns>
    public async Task<Result<ICollection<CurrencyDto>>> GetPagedAsync(CurrencyFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Getting paged currencies with filter: {@Filter}", filter);
        var currencies = await currencyRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);

        var currenciesDto = currencies.ToDto();

        logger.Information("Successfully retrieved {Count} currencies", currenciesDto.Count);
        return Result.Ok(currenciesDto);
    }

    /// <summary>
    /// Создаёт новую валюту
    /// </summary>
    /// <param name="createDto">Данные для создания валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с DTO созданной валюты или ошибкой</returns>
    public async Task<Result<CurrencyDto>> CreateAsync(CreateCurrencyDto createDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Creating currency: {@CreateDto}", createDto);

        if (string.IsNullOrWhiteSpace(createDto.CharCode))
        {
            return Result.Fail(currencyErrorsFactory.CharCodeIsRequired());
        }

        if (string.IsNullOrWhiteSpace(createDto.NumCode))
        {
            return Result.Fail(currencyErrorsFactory.NumCodeIsRequired());
        }

        if (string.IsNullOrWhiteSpace(createDto.Name))
        {
            return Result.Fail(currencyErrorsFactory.NameIsRequired());
        }

        if (!await currencyRepository.IsCharCodeUniqueAsync(createDto.CharCode, cancellationToken: cancellationToken))
        {
            return Result.Fail(currencyErrorsFactory.CharCodeAlreadyExists(createDto.CharCode));
        }

        if (!await currencyRepository.IsNumCodeUniqueAsync(createDto.NumCode, cancellationToken: cancellationToken))
        {
            return Result.Fail(currencyErrorsFactory.NumCodeAlreadyExists(createDto.NumCode));
        }

        var currency = await currencyRepository.AddAsync(createDto.ToCurrency(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Successfully created currency: {CurrencyId} with char code: {CharCode}",
            currency.Id, createDto.CharCode);

        return Result.Ok(currency.ToDto());
    }

    /// <summary>
    /// Обновляет данные существующей валюты
    /// </summary>
    /// <param name="updateDto">Данные для обновления валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с DTO обновленной валюты или ошибкой</returns>
    public async Task<Result<CurrencyDto>> UpdateAsync(UpdateCurrencyDto updateDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Updating currency: {@UpdateDto}", updateDto);

        var currency = await currencyRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (currency is null)
        {
            return Result.Fail(currencyErrorsFactory.NotFound(updateDto.Id));
        }

        var isNeedUpdate = false;

        if (updateDto.Name is not null && !string.Equals(currency.Name, updateDto.Name))
        {
            currency.Name = updateDto.Name;
            isNeedUpdate = true;
        }

        if (updateDto.CharCode is not null && !string.Equals(currency.CharCode, updateDto.CharCode))
        {
            if (!await currencyRepository.IsCharCodeUniqueAsync(updateDto.CharCode, updateDto.Id, cancellationToken))
            {
                return Result.Fail(currencyErrorsFactory.CharCodeAlreadyExists(updateDto.CharCode));
            }

            currency.CharCode = updateDto.CharCode;
            isNeedUpdate = true;
        }

        if (updateDto.NumCode is not null && !string.Equals(currency.NumCode, updateDto.NumCode))
        {
            if (!await currencyRepository.IsNumCodeUniqueAsync(updateDto.NumCode, updateDto.Id, cancellationToken))
            {
                return Result.Fail(currencyErrorsFactory.NumCodeAlreadyExists(updateDto.NumCode));
            }

            currency.NumCode = updateDto.NumCode;
            isNeedUpdate = true;
        }

        if (updateDto.Sign is not null && !string.Equals(currency.Sign, updateDto.Sign))
        {
            currency.Sign = updateDto.Sign;
            isNeedUpdate = true;
        }

        if (updateDto.Emoji is not null && !string.Equals(currency.Emoji, updateDto.Emoji))
        {
            currency.Emoji = updateDto.Emoji;
            isNeedUpdate = true;
        }

        if (isNeedUpdate)
        {
            // нам не нужно вызывать метод currencyRepository.UpdateAsync(), так как сущность currency уже отслеживается
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Successfully updated currency: {CurrencyId}", updateDto.Id);
        }
        else
        {
            logger.Information("No changes detected for currency: {CurrencyId}", updateDto.Id);
        }

        return Result.Ok(currency.ToDto());
    }

    /// <summary>
    /// Помечает валюту как удалённую (soft delete) по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат выполнения операции</returns>
    public async Task<Result> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Soft deleting currency: {CurrencyId}", id);

        var currency =
            await currencyRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (currency is null)
        {
            return Result.Fail(currencyErrorsFactory.NotFound(id));
        }

        currency.MarkAsDeleted();
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Successfully soft deleted currency: {CurrencyId}", id);
        return Result.Ok();
    }

    /// <summary>
    /// Восстанавливает валюту по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат выполнения операции</returns>
    public async Task<Result> RestoreAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Restoring currency: {CurrencyId}", id);

        var currency = await currencyRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (currency is null)
        {
            return Result.Fail(currencyErrorsFactory.NotFound(id));
        }

        if (!currency.IsDeleted)
        {
            logger.Information("Currency is not deleted, nothing to restore: {CurrencyId}", id);
            return Result.Ok();
        }

        currency.Restore();
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Successfully restored currency: {CurrencyId}", id);
        return Result.Ok();
    }

    /// <summary>
    /// Удаляет валюту по идентификатору (жёсткое удаление)
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат выполнения операции</returns>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Deleting currency: {CurrencyId}", id);

        if (!await currencyRepository.CanBeDeletedAsync(id, cancellationToken))
        {
            return Result.Fail(currencyErrorsFactory.CannotDeleteUsedCurrency(id));
        }

        await currencyRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows == 0)
        {
            logger.Warning("No currency was deleted for id: {CurrencyId}", id);
        }

        return Result.Ok();
    }
}