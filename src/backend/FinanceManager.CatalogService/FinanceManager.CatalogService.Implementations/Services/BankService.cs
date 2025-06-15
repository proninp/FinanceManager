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

    public async Task<Result<BankDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.Debug("Getting bank by id: {BankId}", id);

        var bank =
            await _bankRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (bank is null)
        {
            return GetBankNotFoundResult(id);
        }

        _logger.Debug("Successfully retrieved bank: {BankId}", id);
        return Result.Ok(bank.ToDto());
    }

    public async Task<Result<ICollection<BankDto>>> GetPagedAsync(BankFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        _logger.Debug("Getting paged banks with filter: {@Filter}", filter);
        var banks =
            await _bankRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);

        var banksDto = banks.ToDto();

        _logger.Debug("Successfully retrieved {Count} banks", banksDto.Count);
        return Result.Ok(banksDto);
    }

    public async Task<Result<ICollection<BankDto>>> GetAllAsync(bool includeRelated = true,
        CancellationToken cancellationToken = default)
    {
        _logger.Debug("Getting all banks");
        var banks =
            await _bankRepository.GetAllAsync(cancellationToken: cancellationToken);

        var banksDto = banks.ToDto();

        _logger.Debug("Successfully retrieved {Count} banks", banksDto.Count);
        return Result.Ok(banksDto);
    }

    public async Task<Result<BankDto>> CreateAsync(CreateBankDto createDto,
        CancellationToken cancellationToken = default)
    {
        _logger.Debug("Creating bank: {@CreateDto}", createDto);

        // TODO Валидация входящего CreateBankDto на FluentValidation

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

        _logger.Debug("Successfully created bank: {BankId} with name: {Name}",
            bank.Id, createDto.Name);

        return Result.Ok(bank.ToDto());
    }

    public async Task<Result<BankDto>> UpdateAsync(UpdateBankDto updateDto, CancellationToken cancellationToken = default)
    {
        _logger.Debug("Updating bank: {@UpdateDto}", updateDto);

        var bank =
            await _bankRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (bank is null)
        {
            return GetBankNotFoundResult(updateDto.Id);
        }
        
        var updatedProperties = new List<Expression<Func<Bank, object>>>();
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
            updatedProperties.Add(b => b.CountryId);
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
            updatedProperties.Add(c => c.Name);
            isNeedUpdate = true;
        }
        
        if (isNeedUpdate)
        {
            _bankRepository.UpdatePartial(bank, updatedProperties.ToArray());
            await _unitOfWork.CommitAsync(cancellationToken);
            _logger.Debug("Successfully updated bank: {BankId}", updateDto.Id);
        }
        else
        {
            _logger.Debug("No changes detected for country: {BankId}", updateDto.Id);
        }

        return Result.Ok(bank.ToDto());
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.Debug("Deleting bank: {BankId}", id);

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

    public async Task<Result<int>> GetAccountsCountAsync(Guid bankId, bool includeArchivedAccounts = false,
        bool includeDeletedAccounts = false,
        CancellationToken cancellationToken = default)
    {
        _logger.Debug(
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

        _logger.Debug("Accounts count for bank {BankId}: {Count}", bankId, count);
        return Result.Ok(count);
    }

    private Result GetBankNotFoundResult(Guid id)
    {
        _logger.Warning("Bank not found: {BankId}", id);
        return Result.Fail(BankErrorsFactory.NotFound(id));
    }

    private Result GetCountryNotFoundResult(Guid countryId, string bankName)
    {
        _logger.Warning("For bank '{BankName}' country not found: '{CountryId}'.", countryId,
            bankName);
        return Result.Fail(CountryErrorsFactory.NotFound(countryId));
    }

    private async Task<bool> CheckBankNameUniq(string name, Guid countryId, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var isNameUnique =
            await _bankRepository.IsNameUniqueByCountryAsync(name, countryId,
                cancellationToken: cancellationToken);
        return isNameUnique;
    }

    private Result GetBankNameAlreadyExistsResult(string bankName, Guid countryId, string countryName)
    {
        _logger.Warning("Bank name already exists: {Name} for country with id '{CountryId}'", bankName,
            countryId);
        return Result.Fail(BankErrorsFactory.NameAlreadyExists(bankName, countryName));
    }
}