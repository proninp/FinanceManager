using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.Accounts;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Services;

/// <summary>
/// Сервис для управления банковскими счетами
/// </summary>
public class AccountService(
    IUnitOfWork unitOfWork,
    IAccountRepository accountRepository,
    IRegistryHolderRepository registryHolderRepository,
    IAccountTypeRepository accountTypeRepository,
    ICurrencyRepository currencyRepository,
    IBankRepository bankRepository,
    IAccountErrorsFactory errorsFactory,
    ILogger logger) : IAccountService
{
    /// <summary>
    /// Получает счет по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>DTO счета или ошибка, если не найден</returns>
    public async Task<Result<AccountDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Getting account by id: {AccountId}", id);
        var account =
            await accountRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (account is null)
        {
            return Result.Fail(errorsFactory.NotFound(id));
        }

        logger.Information("Successfully retrieved account: {AccountId}", id);
        return Result.Ok(account.ToDto());
    }

    /// <summary>
    /// Получает список счетов с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком счетов или ошибкой</returns>
    public async Task<Result<ICollection<AccountDto>>> GetPagedAsync(AccountFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Getting paged accounts with filter: {@Filter}", filter);
        var accounts = await accountRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);
        var accountsDto = accounts.ToDto();
        logger.Information("Successfully retrieved {Count} account types", accountsDto.Count);
        return Result.Ok(accountsDto);
    }

    /// <summary>
    /// Получает счет по умолчанию пользователя
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со счетом по умолчанию или ошибкой</returns>
    public async Task<Result<AccountDto>> GetDefaultAccountAsync(Guid registryHolderId,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Getting default account for registryHolder: {RegistryHolderId}", registryHolderId);
        var account = await accountRepository.GetDefaultAccountAsync(registryHolderId, cancellationToken);
        if (account is null)
        {
            return Result.Fail(errorsFactory.DefaultAccountNotFound(registryHolderId));
        }

        logger.Information(
            "Successfully get default account: {DefaultAccountId} for registryHolder: {RegistryHolderId}",
            account.Id, registryHolderId);
        return Result.Ok(account.ToDto());
    }

    /// <summary>
    /// Создает новый счет
    /// </summary>
    /// <param name="createDto">Данные для создания счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданным счетом или ошибкой</returns>
    public async Task<Result<AccountDto>> CreateAsync(CreateAccountDto createDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Creating account: {@CreateDto}", createDto);

        if (string.IsNullOrWhiteSpace(createDto.Name))
        {
            return Result.Fail(errorsFactory.NameIsRequired());
        }

        var checkResult = await CheckRegistryHolderAsync(createDto.RegistryHolderId, cancellationToken);
        if (checkResult.IsFailed)
            return Result.Fail(checkResult.Errors);
        checkResult = await CheckAccountTypeAsync(createDto.AccountTypeId, cancellationToken);
        if (checkResult.IsFailed)
            return Result.Fail(checkResult.Errors);
        checkResult = await CheckCurrencyAsync(createDto.CurrencyId, cancellationToken);
        if (checkResult.IsFailed)
            return Result.Fail(checkResult.Errors);
        checkResult = await CheckBankAsync(createDto.BankId, cancellationToken);
        if (checkResult.IsFailed)
            return Result.Fail(checkResult.Errors);

        if (createDto.IsDefault)
        {
            await UnsetDefaultAccountIfExistsAsync(createDto.RegistryHolderId, cancellationToken);
        }

        var account = await accountRepository.AddAsync(createDto.ToAccount(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        logger.Information("Successfully created account: {AccountId}", account.Id);
        return Result.Ok(account.ToDto());
    }

    /// <summary>
    /// Обновляет существующий счет
    /// </summary>
    /// <param name="updateDto">Данные для обновления счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленным счетом или ошибкой</returns>
    public async Task<Result<AccountDto>> UpdateAsync(UpdateAccountDto updateDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Updating account: {@UpdateDto}", updateDto);

        var account = await accountRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (account is null)
        {
            return Result.Fail(errorsFactory.NotFound(updateDto.Id));
        }

        var isDefault = updateDto.IsDefault ?? account.IsDefault;
        var isArchived = updateDto.IsArchived ?? account.IsArchived;
        if (isDefault && (isArchived || account.IsDeleted))
        {
            return Result.Fail(errorsFactory.CannotArchiveDefaultAccount(updateDto.Id));
        }

        var isNeedUpdate = false;

        Result checkResult;
        if (updateDto.AccountTypeId is not null && account.AccountTypeId != updateDto.AccountTypeId.Value)
        {
            checkResult = await CheckAccountTypeAsync(updateDto.AccountTypeId.Value, cancellationToken);
            if (checkResult.IsFailed)
                return Result.Fail(checkResult.Errors);
            account.AccountTypeId = updateDto.AccountTypeId.Value;
            isNeedUpdate = true;
        }

        if (updateDto.CurrencyId is not null && account.CurrencyId != updateDto.CurrencyId.Value)
        {
            checkResult = await CheckCurrencyAsync(updateDto.CurrencyId.Value, cancellationToken);
            if (checkResult.IsFailed)
                return Result.Fail(checkResult.Errors);
            account.CurrencyId = updateDto.CurrencyId.Value;
            isNeedUpdate = true;
        }

        if (updateDto.BankId is not null && account.BankId != updateDto.BankId.Value)
        {
            checkResult = await CheckBankAsync(updateDto.BankId.Value, cancellationToken);
            if (checkResult.IsFailed)
                return Result.Fail(checkResult.Errors);
            account.BankId = updateDto.BankId.Value;
            isNeedUpdate = true;
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Name) && account.Name != updateDto.Name)
        {
            account.Name = updateDto.Name;
            isNeedUpdate = true;
        }

        if (updateDto.IsIncludeInBalance is not null &&
            account.IsIncludeInBalance != updateDto.IsIncludeInBalance.Value)
        {
            account.IsIncludeInBalance = updateDto.IsIncludeInBalance.Value;
            isNeedUpdate = true;
        }

        if (updateDto.IsDefault is not null && account.IsDefault != updateDto.IsDefault.Value)
        {
            if (updateDto.IsDefault.Value)
            {
                await UnsetDefaultAccountIfExistsAsync(account.RegistryHolderId, cancellationToken);
            }

            account.IsDefault = updateDto.IsDefault.Value;
            isNeedUpdate = true;
        }

        if (updateDto.IsArchived is not null && account.IsArchived != updateDto.IsArchived.Value)
        {
            account.IsArchived = updateDto.IsArchived.Value;
            isNeedUpdate = true;
        }

        if (updateDto.CreditLimit is not null && account.CreditLimit != updateDto.CreditLimit.Value)
        {
            // TODO добавить валидацию > 0 при помощи FluentValidation
            account.CreditLimit = updateDto.CreditLimit.Value;
            isNeedUpdate = true;
        }

        if (isNeedUpdate)
        {
            accountRepository.Update(account);
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Successfully updated account: {AccountId}", updateDto.Id);
        }
        else
        {
            logger.Information("No changes detected for account: {AccountId}", updateDto.Id);
        }

        return Result.Ok(account.ToDto());
    }

    /// <summary>
    /// Удаляет счет
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Deleting account: {AccountId}", id);
        
        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            return Result.Ok();
        }

        if (account.IsDefault)
        {
            return Result.Fail(errorsFactory.CannotDeleteDefaultAccount(id));
        }
        
        if (!await accountRepository.CanBeDeletedAsync(id, cancellationToken))
        {
            return Result.Fail(errorsFactory.CannotDeleteUsedAccount(id));
        }

        await accountRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return Result.Ok();
    }

    /// <summary>
    /// Архивирует счет
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> ArchiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Archiving account: {AccountId}", id);
        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (account.IsDefault)
        {
            return Result.Fail(errorsFactory.CannotArchiveDefaultAccount(id));
        }

        if (account.IsArchived)
        {
            return Result.Ok();
        }
        
        await accountRepository.ArchiveAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        logger.Information("Successfully archived account: {AccountId}", id);
        return Result.Ok();
    }

    /// <summary>
    /// Разархивирует счет
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> UnarchiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Unarchiving account: {AccountId}", id);
        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (!account.IsArchived)
        {
            logger.Information("Account with ID {AccountId} is already unarchived. No action taken.", id);
            return Result.Ok();
        }
        
        await accountRepository.UnarchiveAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        logger.Information("Successfully unarchived account: {AccountId}", id);
        return Result.Ok();
    }

    /// <summary>
    /// Устанавливает счет как счет по умолчанию
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> SetAsDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Setting account as default: {AccountId}", id);
        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (account.IsDefault)
        {
            logger.Information("Account {AccountId} is already set as default", id);
            return Result.Ok();
        }

        if (account.IsArchived || account.IsDeleted)
        {
            return Result.Fail(errorsFactory.AccountCannotBeSetAsDefaultIfArchivedOrDeleted(id));
        }

        await accountRepository.SetAsDefaultAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return Result.Ok();
    }

    /// <summary>
    /// Снимает флаг "по умолчанию" со счета и устанавливает другой счет по умолчанию
    /// </summary>
    /// <param name="id">Идентификатор счета, с которого снимается флаг</param>
    /// <param name="replacementDefaultAccountId">Идентификатор нового счета по умолчанию</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> UnsetAsDefaultAsync(Guid id, Guid replacementDefaultAccountId,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Unsetting account as default: {AccountId}", id);
        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (!account.IsDefault)
        {
            return Result.Ok();
        }

        logger.Information("Getting new account fo replace as default: {AccountId}", replacementDefaultAccountId);
        var replacementAccount =
            await accountRepository.GetByIdAsync(replacementDefaultAccountId, cancellationToken: cancellationToken);
        if (replacementAccount is null)
        {
            return Result.Fail(errorsFactory.ReplacementDefaultAccountNotFound(replacementDefaultAccountId));
        }
        
        if (replacementAccount.IsArchived || replacementAccount.IsDeleted)
        {
            return Result.Fail(errorsFactory.ReplacementAccountCannotBeSetAsDefault(replacementDefaultAccountId));
        }

        if (replacementAccount.RegistryHolderId != account.RegistryHolderId)
        {
            return Result.Fail(errorsFactory.RegistryHolderDiffersBetweenReplacedDefaultAccounts(id, replacementDefaultAccountId));
        }

        account.IsDefault = false;
        replacementAccount.IsDefault = true;
        
        accountRepository.Update(account);
        await unitOfWork.CommitAsync(cancellationToken);
        logger.Information("Successfully unset default for account: {AccountId}", id);
        return Result.Ok();
    }

    /// <summary>
    /// Проверяет существование владельца счета
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки</returns>
    protected async Task<Result> CheckRegistryHolderAsync(Guid registryHolderId, CancellationToken cancellationToken)
    {
        var holder = await registryHolderRepository.GetByIdAsync(registryHolderId, disableTracking: true,
            cancellationToken: cancellationToken);
        return holder is null ? Result.Fail(errorsFactory.RegistryHolderNotFound(registryHolderId)) : Result.Ok();
    }

    /// <summary>
    /// Проверяет существование и актуальность типа счета
    /// </summary>
    /// <param name="accountTypeId">Идентификатор типа счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки</returns>
    protected async Task<Result> CheckAccountTypeAsync(Guid accountTypeId, CancellationToken cancellationToken)
    {
        var accountType = await accountTypeRepository.GetByIdAsync(
            accountTypeId, disableTracking: true, cancellationToken: cancellationToken);
        if (accountType is null)
        {
            return Result.Fail(errorsFactory.AccountTypeNotFound(accountTypeId));
        }

        if (accountType.IsDeleted)
        {
            return Result.Fail(errorsFactory.AccountTypeIsSoftDeleted(accountType.Id));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Проверяет существование и актуальность валюты счета
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки</returns>
    protected async Task<Result> CheckCurrencyAsync(Guid currencyId, CancellationToken cancellationToken)
    {
        var currency = await currencyRepository.GetByIdAsync(
            currencyId, disableTracking: true, cancellationToken: cancellationToken);
        if (currency is null)
        {
            return Result.Fail(errorsFactory.CurrencyNotFound(currencyId));
        }

        return currency.IsDeleted ? Result.Fail(errorsFactory.CurrencyIsSoftDeleted(currencyId)) : Result.Ok();
    }

    /// <summary>
    /// Проверяет существование банка
    /// </summary>
    /// <param name="bankId">Идентификатор банка</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки</returns>
    protected async Task<Result> CheckBankAsync(Guid bankId, CancellationToken cancellationToken)
    {
        var bank = await bankRepository.GetByIdAsync(
            bankId, disableTracking: true, cancellationToken: cancellationToken);
        return bank is null ? Result.Fail(errorsFactory.BankNotFound(bankId)) : Result.Ok();
    }

    /// <summary>
    /// Снимает признак "по умолчанию" с предыдущего счета по умолчанию пользователя, если он был установлен
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    protected async Task UnsetDefaultAccountIfExistsAsync(Guid registryHolderId, CancellationToken cancellationToken)
    {
        var previousDefaultAccount =
            await accountRepository.GetDefaultAccountAsync(registryHolderId, cancellationToken);
        if (previousDefaultAccount is not null)
        {
            await accountRepository.UnsetAsDefaultAsync(previousDefaultAccount.Id, cancellationToken);
            logger.Information("Unset default property to false for account: {AccountId}",
                previousDefaultAccount.Id);
        }
    }
}