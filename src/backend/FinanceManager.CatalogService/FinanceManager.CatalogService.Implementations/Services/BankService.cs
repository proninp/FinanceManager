using System.Linq.Expressions;
using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.Banks;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.Implementations.Errors;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Services;

/// <summary>
/// Сервис для управления банками, реализующий основные CRUD-операции и дополнительные бизнес-функции.
/// </summary>
public class BankService(
    IUnitOfWork unitOfWork,
    IBankRepository bankRepository,
    ICountryRepository countryRepository,
    ILogger logger)
    : IBankService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBankRepository _bankRepository = bankRepository;
    private readonly ICountryRepository _countryRepository = countryRepository;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Получает банк по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор банка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с DTO банка или ошибкой, если не найден.</returns>
    public async Task<Result<BankDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.Information("Getting bank by id: {BankId}", id);

        var bank =
            await _bankRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (bank is null)
        {
            return GetBankNotFoundResult(id);
        }

        _logger.Information("Successfully retrieved bank: {BankId}", id);
        return Result.Ok(bank.ToDto());
    }

    /// <summary>
    /// Получает список банков с пагинацией и фильтрацией.
    /// </summary>
    /// <param name="filter">Параметры фильтрации и пагинации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с коллекцией DTO банков.</returns>
    public async Task<Result<ICollection<BankDto>>> GetPagedAsync(BankFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        _logger.Information("Getting paged banks with filter: {@Filter}", filter);
        var banks =
            await _bankRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);

        var banksDto = banks.ToDto();

        _logger.Information("Successfully retrieved {Count} banks", banksDto.Count);
        return Result.Ok(banksDto);
    }

    /// <summary>
    /// Получает все банки.
    /// </summary>
    /// <param name="includeRelated">Включать связанные сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с коллекцией DTO банков.</returns>
    public async Task<Result<ICollection<BankDto>>> GetAllAsync(bool includeRelated = true,
        CancellationToken cancellationToken = default)
    {
        _logger.Information("Getting all banks");
        var banks =
            await _bankRepository.GetAllAsync(cancellationToken: cancellationToken);

        var banksDto = banks.ToDto();

        _logger.Information("Successfully retrieved {Count} banks", banksDto.Count);
        return Result.Ok(banksDto);
    }

    /// <summary>
    /// Создаёт новый банк.
    /// </summary>
    /// <param name="createDto">Данные для создания банка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с DTO созданного банка или ошибкой.</returns>
    public async Task<Result<BankDto>> CreateAsync(CreateBankDto createDto,
        CancellationToken cancellationToken = default)
    {
        _logger.Information("Creating bank: {@CreateDto}", createDto);

        if (string.IsNullOrWhiteSpace(createDto.Name))
        {
            _logger.Warning("Bank name is required");
            return Result.Fail(BankErrorsFactory.NameIsRequired());
        }

        var country = await _countryRepository.GetByIdAsync(createDto.CountryId, disableTracking: true,
            cancellationToken: cancellationToken);

        if (country is null)
        {
            return GetCountryNotFoundResult(createDto.CountryId, createDto.Name);
        }

        if (!await CheckBankNameUniq(createDto.Name, createDto.CountryId, cancellationToken: cancellationToken))
        {
            return GetBankNameAlreadyExistsResult(createDto.Name, country.Id, country.Name);
        }

        var bank = await _bankRepository.AddAsync(createDto.ToBank(), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.Information("Successfully created bank: {BankId} with name: {Name}",
            bank.Id, createDto.Name);

        return Result.Ok(bank.ToDto());
    }

    /// <summary>
    /// Обновляет данные существующего банка.
    /// </summary>
    /// <param name="updateDto">Данные для обновления банка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с DTO обновленного банка или ошибкой.</returns>
    public async Task<Result<BankDto>> UpdateAsync(UpdateBankDto updateDto, CancellationToken cancellationToken = default)
    {
        _logger.Information("Updating bank: {@UpdateDto}", updateDto);

        var bank =
            await _bankRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (bank is null)
        {
            return GetBankNotFoundResult(updateDto.Id);
        }
        
        var isNeedUpdate = false;
        
        Country? country = null;
        if (updateDto.CountryId is not null && updateDto.CountryId.Value != bank.CountryId)
        {
            country = await _countryRepository.GetByIdAsync(updateDto.CountryId.Value, disableTracking: true,
                cancellationToken: cancellationToken);

            if (country is null)
            {
                return GetCountryNotFoundResult(updateDto.CountryId.Value, bank.Name);
            }
            bank.CountryId = updateDto.CountryId.Value;
            isNeedUpdate = true;
        }

        country ??= bank.Country;
        
        if (updateDto.Name is not null && !string.Equals(updateDto.Name, bank.Name))
        {
            var isNameUnique =
                await _bankRepository.IsNameUniqueByCountryAsync(
                    updateDto.Name, country.Id, cancellationToken: cancellationToken);
            if (!isNameUnique)
            {
                return GetBankNameAlreadyExistsResult(updateDto.Name, country.Id, country.Name);
            }
            
            bank.Name = updateDto.Name;
            isNeedUpdate = true;
        }
        
        if (isNeedUpdate)
        {
            _bankRepository.Update(bank);
            await _unitOfWork.CommitAsync(cancellationToken);
            _logger.Information("Successfully updated bank: {BankId}", updateDto.Id);
        }
        else
        {
            _logger.Information("No changes detected for bank: {BankId}", updateDto.Id);
        }

        return Result.Ok(bank.ToDto());
    }

    /// <summary>
    /// Удаляет банк по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор банка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат выполнения операции.</returns>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.Information("Deleting bank: {BankId}", id);

        if (!await _bankRepository.CanBeDeletedAsync(id, cancellationToken))
        {
            _logger.Warning("Cannot delete bank '{BankId}' because it is using in other entities", id);
            return Result.Fail(BankErrorsFactory.CannotDeleteUsedBank(id));
        }

        await _bankRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await _unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows == 0)
        {
            _logger.Warning("No bank was deleted for id: {BankId}", id);
        }

        return Result.Ok();
    }

    /// <summary>
    /// Получает количество счетов, связанных с банком.
    /// </summary>
    /// <param name="bankId">Идентификатор банка.</param>
    /// <param name="includeArchivedAccounts">Включать архивные счета.</param>
    /// <param name="includeDeletedAccounts">Включать удалённые счета.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с количеством счетов.</returns>
    public async Task<Result<int>> GetAccountsCountAsync(Guid bankId, bool includeArchivedAccounts = false,
        bool includeDeletedAccounts = false,
        CancellationToken cancellationToken = default)
    {
        _logger.Information(
            "Getting accounts count for bank: {BankId} (includeArchived: {IncludeArchived}, includeDeleted: {IncludeDeleted})",
            bankId, includeArchivedAccounts, includeDeletedAccounts);

        var bank = await _bankRepository.GetByIdAsync(bankId, disableTracking: true,
            cancellationToken: cancellationToken);
        if (bank is null)
        {
            return GetBankNotFoundResult(bankId);
        }

        var count = await _bankRepository.GetAccountsCountAsync(
            bankId,
            includeArchivedAccounts,
            includeDeletedAccounts,
            cancellationToken);

        _logger.Information("Accounts count for bank {BankId}: {Count}", bankId, count);
        return Result.Ok(count);
    }

    /// <summary>
    /// Формирует результат ошибки, если банк не найден.
    /// </summary>
    /// <param name="id">Идентификатор банка.</param>
    /// <returns>Результат с ошибкой о ненайденном банке.</returns>
    private Result GetBankNotFoundResult(Guid id)
    {
        _logger.Warning("Bank not found: {BankId}", id);
        return Result.Fail(BankErrorsFactory.NotFound(id));
    }

    /// <summary>
    /// Формирует результат ошибки, если страна не найдена для указанного банка.
    /// </summary>
    /// <param name="countryId">Идентификатор страны.</param>
    /// <param name="bankName">Название банка.</param>
    /// <returns>Результат с ошибкой о ненайденной стране.</returns>
    private Result GetCountryNotFoundResult(Guid countryId, string bankName)
    {
        _logger.Warning("For bank '{BankName}' country not found: '{CountryId}'.", countryId,
            bankName);
        return Result.Fail(CountryErrorsFactory.NotFound(countryId));
    }

    /// <summary>
    /// Проверяет уникальность названия банка в рамках страны.
    /// </summary>
    /// <param name="name">Название банка.</param>
    /// <param name="countryId">Идентификатор страны.</param>
    /// <param name="excludeId">Идентификатор банка, который нужно исключить из проверки (опционально).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если название уникально; иначе false.</returns>
    private async Task<bool> CheckBankNameUniq(string name, Guid countryId, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var isNameUnique =
            await _bankRepository.IsNameUniqueByCountryAsync(name, countryId,
                cancellationToken: cancellationToken);
        return isNameUnique;
    }

    /// <summary>
    /// Формирует результат ошибки, если банк с таким названием уже существует в стране.
    /// </summary>
    /// <param name="bankName">Название банка.</param>
    /// <param name="countryId">Идентификатор страны.</param>
    /// <param name="countryName">Название страны.</param>
    /// <returns>Результат с ошибкой о неуникальном названии банка.</returns>
    private Result GetBankNameAlreadyExistsResult(string bankName, Guid countryId, string countryName)
    {
        _logger.Warning("Bank name already exists: {Name} for country with id '{CountryId}'", bankName,
            countryId);
        return Result.Fail(BankErrorsFactory.NameAlreadyExists(bankName, countryName));
    }
}