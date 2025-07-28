using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.Countries;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Services;

/// <summary>
/// Сервис для управления справочником стран, реализующий основные CRUD-операции
/// </summary>
public class CountryService(
    IUnitOfWork unitOfWork,
    ICountryRepository countryRepository,
    ICountryErrorsFactory countryErrorsFactory,
    ILogger logger)
    : ICountryService
{
    /// <summary>
    /// Получает страну по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор страны</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с DTO страны или ошибкой, если не найдена</returns>
    public async Task<Result<CountryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Получение страны по идентификатору: {CountryId}", id);

        var country =
            await countryRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (country is null)
        {
            logger.Warning("Страна с идентификатором {CountryId} не найдена", id);
            return Result.Fail(countryErrorsFactory.NotFound(id));
        }

        logger.Information("Страна {CountryId} успешно получена", id);
        return Result.Ok(country.ToDto());
    }

    /// <summary>
    /// Получает список стран с пагинацией и фильтрацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации и пагинации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с коллекцией DTO стран</returns>
    public async Task<Result<ICollection<CountryDto>>> GetPagedAsync(CountryFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Получение списка стран с фильтрацией: {@Filter}", filter);
        
        var countries =
            await countryRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);

        var countriesDto = countries.ToDto();

        logger.Information("Получено {Count} стран", countriesDto.Count);
        return Result.Ok(countriesDto);
    }

    /// <summary>
    /// Создает новую страну
    /// </summary>
    /// <param name="createDto">Данные для создания страны</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с DTO созданной страны или ошибкой</returns>
    public async Task<Result<CountryDto>> CreateAsync(CreateCountryDto createDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Создание новой страны: {@CreateDto}", createDto);

        if (string.IsNullOrWhiteSpace(createDto.Name))
        {
            logger.Warning("Попытка создания страны без указания названия");
            return Result.Fail(countryErrorsFactory.NameIsRequired());
        }

        var isNameUnique =
            await countryRepository.IsNameUniqueAsync(createDto.Name, cancellationToken: cancellationToken);
        if (!isNameUnique)
        {
            logger.Warning("Попытка создания страны с неуникальным названием: '{CountryName}'", createDto.Name);
            return Result.Fail(countryErrorsFactory.NameAlreadyExists(createDto.Name));
        }

        var country = await countryRepository.AddAsync(createDto.ToCountry(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Страна {CountryId} с названием '{CountryName}' успешно создана",
            country.Id, createDto.Name);

        return Result.Ok(country.ToDto());
    }

    /// <summary>
    /// Обновляет данные существующей страны
    /// </summary>
    /// <param name="updateDto">Данные для обновления страны</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с DTO обновленной страны или ошибкой</returns>
    public async Task<Result<CountryDto>> UpdateAsync(UpdateCountryDto updateDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Обновление страны: {@UpdateDto}", updateDto);

        var country =
            await countryRepository.GetByIdAsync(updateDto.Id, true, cancellationToken: cancellationToken);
        if (country is null)
        {
            logger.Warning("Страна с идентификатором {CountryId} не найдена для обновления", updateDto.Id);
            return Result.Fail(countryErrorsFactory.NotFound(updateDto.Id));
        }

        var isNeedUpdate = false;

        if (!string.Equals(country.Name, updateDto.Name))
        {
            var isNameUnique =
                await countryRepository.IsNameUniqueAsync(updateDto.Name, updateDto.Id,
                    cancellationToken: cancellationToken);
            if (!isNameUnique)
            {
                logger.Warning("Попытка обновления страны {CountryId} с неуникальным названием: '{CountryName}'", 
                    updateDto.Id, updateDto.Name);
                return Result.Fail(countryErrorsFactory.NameAlreadyExists(updateDto.Name));
            }

            country.Name = updateDto.Name;
            isNeedUpdate = true;
        }

        if (isNeedUpdate)
        {
            // нам не нужно вызывать метод countryRepository.UpdateAsync(), так как сущность country уже отслеживается
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Страна {CountryId} успешно обновлена", updateDto.Id);
        }
        else
        {
            logger.Information("Изменения для страны {CountryId} не обнаружены", updateDto.Id);
        }
        
        return Result.Ok(country.ToDto());
    }

    /// <summary>
    /// Удаляет страну по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор страны</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат выполнения операции</returns>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Удаление страны: {CountryId}", id);

        if (!await countryRepository.CanBeDeletedAsync(id, cancellationToken))
        {
            logger.Warning("Страна {CountryId} не может быть удалена, так как используется в банках", id);
            return Result.Fail(countryErrorsFactory.CannotDeleteUsedCountry(id));
        }
        
        await countryRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows == 0)
        {
            logger.Warning("Страна {CountryId} не была удалена, возможно она не существует", id);
        }
        else
        {
            logger.Information("Страна {CountryId} успешно удалена", id);
        }
        
        return Result.Ok();
    }
}