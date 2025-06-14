using System.Linq.Expressions;
using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.Countries;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.Implementations.Errors;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Services;

/// <summary>
/// Сервис для управления справочником стран, реализующий основные CRUD-операции
/// </summary>
public class CountryService(IUnitOfWork unitOfWork, ICountryRepository countryRepository, ILogger logger)
    : ICountryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICountryRepository _countryRepository = countryRepository;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Получает страну по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор страны</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с DTO страны или ошибкой, если не найдена</returns>
    public async Task<Result<CountryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.Debug("Getting country by id: {CountryId}", id);

        var country =
            await _countryRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (country is null)
        {
            _logger.Warning("Country not found: {CountryId}", id);
            return Result.Fail(CountryErrorsFactory.NotFound(id));
        }

        _logger.Debug("Successfully retrieved country: {CountryId}", id);
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
        _logger.Debug("Getting paged countries with filter: {@Filter}", filter);
        var countries =
            await _countryRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);

        var countriesDto = countries.ToDto();

        _logger.Debug("Successfully retrieved {Count} countries", countriesDto.Count);
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
        _logger.Debug("Creating country: {@CreateDto}", createDto);

        // TODO Валидация входящего CreateCountryDto на FluentValidation

        var isNameUnique =
            await _countryRepository.IsNameUniqueAsync(createDto.Name, cancellationToken: cancellationToken);
        if (!isNameUnique)
        {
            _logger.Warning("Country name already exists: {Name}", createDto.Name);
            return Result.Fail(CountryErrorsFactory.NameAlreadyExists(createDto.Name));
        }

        var country = await _countryRepository.AddAsync(createDto.ToCountry(), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.Debug("Successfully created country: {CountryId} with name: {Name}",
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
        _logger.Debug("Updating country: {@UpdateDto}", updateDto);

        var country =
            await _countryRepository.GetByIdAsync(updateDto.Id, true, cancellationToken: cancellationToken);
        if (country is null)
        {
            _logger.Warning("Country not found for update: {CountryId}", updateDto.Id);
            return Result.Fail(CountryErrorsFactory.NotFound(updateDto.Id));
        }

        var updatedProperties = new List<Expression<Func<Country, object>>>();
        var isNeedUpdate = false;

        if (updateDto.Name is not null && !string.Equals(updateDto.Name, country.Name))
        {
            var isNameUnique =
                await _countryRepository.IsNameUniqueAsync(updateDto.Name, cancellationToken: cancellationToken);
            if (!isNameUnique)
            {
                _logger.Warning("Country name already exists: {Name}", updateDto.Name);
                return Result.Fail(CountryErrorsFactory.NameAlreadyExists(updateDto.Name));
            }
            
            country.Name = updateDto.Name;
            updatedProperties.Add(c => c.Name);
            isNeedUpdate = true;
        }
        
        if (isNeedUpdate)
        {
            _countryRepository.UpdatePartial(country, updatedProperties.ToArray());
            await _unitOfWork.CommitAsync(cancellationToken);
            _logger.Debug("Successfully updated country: {CountryId}", updateDto.Id);
        }
        else
        {
            _logger.Debug("No changes detected for country: {CountryId}", updateDto.Id);
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
        _logger.Debug("Deleting country: {CountryId}", id);
        await _countryRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await _unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows == 0)
        {
            _logger.Warning("No country was deleted for id: {CountryId}", id);
        }
        return Result.Ok();
    }
}