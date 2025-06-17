using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.Banks;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
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
    IBankErrorsFactory bankErrorsFactory,
    ICountryErrorsFactory countryErrorsFactory,
    ILogger logger)
    : IBankService
{
    /// <summary>
    /// Получает банк по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор банка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат с DTO банка или ошибкой, если не найден.</returns>
    public async Task<Result<BankDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Getting bank by id: {BankId}", id);

        var bank =
            await bankRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (bank is null)
        {
            return GetBankNotFoundResult(id);
        }

        logger.Information("Successfully retrieved bank: {BankId}", id);
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
        logger.Information("Getting paged banks with filter: {@Filter}", filter);
        var banks =
            await bankRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);

        var banksDto = banks.ToDto();

        logger.Information("Successfully retrieved {Count} banks", banksDto.Count);
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
        logger.Information("Getting all banks");
        var banks =
            await bankRepository.GetAllAsync(cancellationToken: cancellationToken);

        var banksDto = banks.ToDto();

        logger.Information("Successfully retrieved {Count} banks", banksDto.Count);
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
        logger.Information("Creating bank: {@CreateDto}", createDto);

        if (string.IsNullOrWhiteSpace(createDto.Name))
        {
            logger.Warning("Bank name is required");
            return Result.Fail(bankErrorsFactory.NameIsRequired());
        }

        var country = await countryRepository.GetByIdAsync(createDto.CountryId, disableTracking: true,
            cancellationToken: cancellationToken);

        if (country is null)
        {
            return GetCountryNotFoundResult(createDto.CountryId, createDto.Name);
        }

        if (!await CheckBankNameUniq(createDto.Name, createDto.CountryId, cancellationToken: cancellationToken))
        {
            return GetBankNameAlreadyExistsResult(createDto.Name, country.Id, country.Name);
        }

        var bank = await bankRepository.AddAsync(createDto.ToBank(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Successfully created bank: {BankId} with name: {Name}",
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
        logger.Information("Updating bank: {@UpdateDto}", updateDto);

        var bank =
            await bankRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (bank is null)
        {
            return GetBankNotFoundResult(updateDto.Id);
        }
        
        var isNeedUpdate = false;
        
        Country? country = null;
        if (updateDto.CountryId is not null && updateDto.CountryId.Value != bank.CountryId)
        {
            country = await countryRepository.GetByIdAsync(updateDto.CountryId.Value, disableTracking: true,
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
                await bankRepository.IsNameUniqueByCountryAsync(
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
            bankRepository.Update(bank);
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Successfully updated bank: {BankId}", updateDto.Id);
        }
        else
        {
            logger.Information("No changes detected for bank: {BankId}", updateDto.Id);
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
        logger.Information("Deleting bank: {BankId}", id);

        if (!await bankRepository.CanBeDeletedAsync(id, cancellationToken))
        {
            logger.Warning("Cannot delete bank '{BankId}' because it is using in other entities", id);
            return Result.Fail(bankErrorsFactory.CannotDeleteUsedBank(id));
        }

        await bankRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows == 0)
        {
            logger.Warning("No bank was deleted for id: {BankId}", id);
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
        logger.Information(
            "Getting accounts count for bank: {BankId} (includeArchived: {IncludeArchived}, includeDeleted: {IncludeDeleted})",
            bankId, includeArchivedAccounts, includeDeletedAccounts);

        var bank = await bankRepository.GetByIdAsync(bankId, disableTracking: true,
            cancellationToken: cancellationToken);
        if (bank is null)
        {
            return GetBankNotFoundResult(bankId);
        }

        var count = await bankRepository.GetAccountsCountAsync(
            bankId,
            includeArchivedAccounts,
            includeDeletedAccounts,
            cancellationToken);

        logger.Information("Accounts count for bank {BankId}: {Count}", bankId, count);
        return Result.Ok(count);
    }

    /// <summary>
    /// Формирует результат ошибки, если банк не найден.
    /// </summary>
    /// <param name="id">Идентификатор банка.</param>
    /// <returns>Результат с ошибкой о ненайденном банке.</returns>
    private Result GetBankNotFoundResult(Guid id)
    {
        logger.Warning("Bank not found: {BankId}", id);
        return Result.Fail(bankErrorsFactory.NotFound(id));
    }

    /// <summary>
    /// Формирует результат ошибки, если страна не найдена для указанного банка.
    /// </summary>
    /// <param name="countryId">Идентификатор страны.</param>
    /// <param name="bankName">Название банка.</param>
    /// <returns>Результат с ошибкой о ненайденной стране.</returns>
    private Result GetCountryNotFoundResult(Guid countryId, string bankName)
    {
        logger.Warning("For bank '{BankName}' country not found: '{CountryId}'.", countryId,
            bankName);
        return Result.Fail(countryErrorsFactory.NotFound(countryId));
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
            await bankRepository.IsNameUniqueByCountryAsync(name, countryId,
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
        logger.Warning("Bank name already exists: {Name} for country with id '{CountryId}'", bankName,
            countryId);
        return Result.Fail(bankErrorsFactory.NameAlreadyExists(bankName, countryName));
    }
}