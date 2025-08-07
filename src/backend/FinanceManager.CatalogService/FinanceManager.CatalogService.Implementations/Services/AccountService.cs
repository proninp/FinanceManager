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
        logger.Information("Получение счета по идентификатору: {AccountId}", id);
        var account =
            await accountRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (account is null)
        {
            logger.Warning("Счет с идентификатором {AccountId} не найден", id);
            return Result.Fail(errorsFactory.NotFound(id));
        }

        logger.Information("Счет {AccountId} успешно получен", id);
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
        logger.Information("Получение списка счетов с фильтрацией: {@Filter}", filter);

        var accounts = await accountRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);
        var accountsDto = accounts.ToDto();

        logger.Information("Получено {Count} счетов", accountsDto.Count);
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
        logger.Information("Получение счета по умолчанию для владельца: {RegistryHolderId}", registryHolderId);
        var account = await accountRepository.GetDefaultAccountAsync(registryHolderId, cancellationToken);
        if (account is null)
        {
            logger.Warning("Счет по умолчанию для владельца {RegistryHolderId} не найден", registryHolderId);
            return Result.Fail(errorsFactory.DefaultAccountNotFound(registryHolderId));
        }

        logger.Information("Найден счет по умолчанию {AccountId} для владельца {RegistryHolderId}",
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
        logger.Information("Создание нового счета: {@CreateDto}", createDto);

        if (string.IsNullOrWhiteSpace(createDto.Name))
        {
            logger.Warning("Попытка создания счета без указания имени");
            return Result.Fail(errorsFactory.NameIsRequired());
        }

        // TODO валидация входящих значений createDto при помощи FluentValidation

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
            logger.Debug("Снятие флага по умолчанию с предыдущего счета для владельца {RegistryHolderId}",
                createDto.RegistryHolderId);
            await UnsetDefaultAccountIfExistsAsync(createDto.RegistryHolderId, cancellationToken);
        }

        var account = await accountRepository.AddAsync(createDto.ToAccount(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Счет {AccountId} успешно создан", account.Id);
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
        logger.Information("Обновление счета: {@UpdateDto}", updateDto);

        var account = await accountRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (account is null)
        {
            logger.Warning("Счет с идентификатором {AccountId} не найден для обновления", updateDto.Id);
            return Result.Fail(errorsFactory.NotFound(updateDto.Id));
        }

        var isDefault = updateDto.IsDefault ?? account.IsDefault;
        var isArchived = updateDto.IsArchived ?? account.IsArchived;
        if (isDefault && (isArchived || account.IsDeleted))
        {
            logger.Warning("Попытка архивирования счета по умолчанию {AccountId}", updateDto.Id);
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
                logger.Debug("Снятие флага по умолчанию с предыдущего счета для владельца {RegistryHolderId}",
                    account.RegistryHolderId);
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
            // TODO добавить валидацию updateDto.CreditLimit > 0 при помощи FluentValidation
            account.CreditLimit = updateDto.CreditLimit.Value;
            isNeedUpdate = true;
        }

        if (isNeedUpdate)
        {
            // нам не нужно вызывать метод accountRepository.UpdateAsync(), так как сущность account уже отслеживается
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Счет {AccountId} успешно обновлен", updateDto.Id);
        }
        else
        {
            logger.Information("Изменения для счета {AccountId} не обнаружены", updateDto.Id);
        }

        return Result.Ok(account.ToDto());
    }

    /// <summary>
    /// Мягкое удаление (soft delete) счета по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Мягкое удаление счета: {AccountId}", id);

        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            logger.Warning("Счет с идентификатором {AccountId} не найден для удаления", id);
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (account.IsDeleted)
        {
            logger.Information("Счет {AccountId} уже помечен как удаленный", id);
            return Result.Ok();
        }

        if (account.IsDefault)
        {
            logger.Warning("Попытка мягкого удаления счета по умолчанию {AccountId}", id);
            return Result.Fail(errorsFactory.CannotSoftDeleteDefaultAccount(id));
        }

        account.MarkAsDeleted();
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Счет {AccountId} успешно помечен как удаленный", id);
        return Result.Ok();
    }

    /// <summary>
    /// Восстанавливает ранее удалённый (мягко удалённый) счет
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> RestoreDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Восстановление удаленного счета: {AccountId}", id);

        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            logger.Warning("Счет с идентификатором {AccountId} не найден для восстановления", id);
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (!account.IsDeleted)
        {
            logger.Information("Счет {AccountId} не был удален, восстановление не требуется", id);
            return Result.Ok();
        }

        account.Restore();
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Счет {AccountId} успешно восстановлен", id);
        return Result.Ok();
    }

    /// <summary>
    /// Удаляет счет
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Жесткое удаление счета: {AccountId}", id);

        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            logger.Information("Счет {AccountId} не найден, удаление не требуется", id);
            return Result.Ok();
        }

        if (account.IsDefault)
        {
            return Result.Fail(errorsFactory.CannotDeleteDefaultAccount(id));
        }

        await accountRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);

        if (affectedRows > 0)
        {
            logger.Information("Счет {AccountId} успешно удален", id);
        }
        
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
        logger.Information("Архивирование счета: {AccountId}", id);
        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            logger.Warning("Счет с идентификатором {AccountId} не найден для архивирования", id);
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (account.IsDefault)
        {
            logger.Warning("Попытка архивирования счета по умолчанию {AccountId}", id);
            return Result.Fail(errorsFactory.CannotArchiveDefaultAccount(id));
        }

        if (account.IsArchived)
        {
            logger.Information("Счет {AccountId} уже заархивирован", id);
            return Result.Ok();
        }

        account.Archive();
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Счет {AccountId} успешно заархивирован", id);
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
        logger.Information("Разархивирование счета: {AccountId}", id);
        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            logger.Warning("Счет с идентификатором {AccountId} не найден для разархивирования", id);
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (!account.IsArchived)
        {
            logger.Information("Счет {AccountId} не был заархивирован, действие не требуется", id);
            return Result.Ok();
        }

        account.UnArchive();
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Счет {AccountId} успешно разархивирован", id);
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
        logger.Information("Установка счета по умолчанию: {AccountId}", id);
        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            logger.Warning("Счет с идентификатором {AccountId} не найден", id);
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (account.IsDefault)
        {
            logger.Information("Счет {AccountId} уже установлен как счет по умолчанию", id);
            return Result.Ok();
        }

        if (account.IsArchived || account.IsDeleted)
        {
            logger.Warning(
                "Попытка установки заархивированного или удаленного счета {AccountId} как счета по умолчанию", id);
            return Result.Fail(errorsFactory.AccountCannotBeSetAsDefaultIfArchivedOrDeleted(id));
        }

        account.SetAsDefault();
        await unitOfWork.CommitAsync(cancellationToken);
        logger.Information("Счет {AccountId} успешно установлен как счет по умолчанию", id);
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
        logger.Information(
            "Снятие флага по умолчанию со счета {AccountId} и установка флага на счет {ReplacementAccountId}",
            id, replacementDefaultAccountId);
        var account = await accountRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (account is null)
        {
            logger.Warning("Счет с идентификатором {AccountId} не найден", id);
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (!account.IsDefault)
        {
            logger.Information("Счет {AccountId} не был установлен как счет по умолчанию", id);
            return Result.Ok();
        }

        var replacementAccount =
            await accountRepository.GetByIdAsync(replacementDefaultAccountId, cancellationToken: cancellationToken);
        if (replacementAccount is null)
        {
            logger.Warning("Замещающий счет с идентификатором {ReplacementAccountId} не найден",
                replacementDefaultAccountId);
            return Result.Fail(errorsFactory.ReplacementDefaultAccountNotFound(replacementDefaultAccountId));
        }

        if (replacementAccount.IsArchived || replacementAccount.IsDeleted)
        {
            logger.Warning("Замещающий счет {ReplacementAccountId} заархивирован или удален",
                replacementDefaultAccountId);
            return Result.Fail(errorsFactory.ReplacementAccountCannotBeSetAsDefault(replacementDefaultAccountId));
        }

        if (replacementAccount.RegistryHolderId != account.RegistryHolderId)
        {
            logger.Warning("Владельцы счетов {AccountId} и {ReplacementAccountId} не совпадают",
                id, replacementDefaultAccountId);
            return Result.Fail(
                errorsFactory.RegistryHolderDiffersBetweenReplacedDefaultAccounts(id, replacementDefaultAccountId));
        }

        account.UnsetAsDefault();
        replacementAccount.SetAsDefault();
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information(
            "Флаг по умолчанию успешно снят со счета {AccountId} и установлен на счет {ReplacementAccountId}",
            id, replacementDefaultAccountId);
        return Result.Ok();
    }

    /// <summary>
    /// Проверяет существование владельца счета
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки</returns>
    private async Task<Result> CheckRegistryHolderAsync(Guid registryHolderId, CancellationToken cancellationToken)
    {
        logger.Debug("Проверка существования владельца справочника: {RegistryHolderId}", registryHolderId);
        var holder = await registryHolderRepository.GetByIdAsync(registryHolderId, disableTracking: true,
            cancellationToken: cancellationToken);
        if (holder is null)
        {
            logger.Warning("Владелец справочника {RegistryHolderId} не найден", registryHolderId);
            return Result.Fail(errorsFactory.RegistryHolderNotFound(registryHolderId));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Проверяет существование и актуальность типа счета
    /// </summary>
    /// <param name="accountTypeId">Идентификатор типа счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки</returns>
    private async Task<Result> CheckAccountTypeAsync(Guid accountTypeId, CancellationToken cancellationToken)
    {
        logger.Debug("Проверка существования типа счета: {AccountTypeId}", accountTypeId);

        var accountType = await accountTypeRepository.GetByIdAsync(
            accountTypeId, disableTracking: true, cancellationToken: cancellationToken);
        if (accountType is null)
        {
            logger.Warning("Тип счета {AccountTypeId} не найден", accountTypeId);
            return Result.Fail(errorsFactory.AccountTypeNotFound(accountTypeId));
        }

        if (accountType.IsDeleted)
        {
            logger.Warning("Тип счета {AccountTypeId} помечен как удаленный", accountTypeId);
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
    private async Task<Result> CheckCurrencyAsync(Guid currencyId, CancellationToken cancellationToken)
    {
        logger.Debug("Проверка существования валюты: {CurrencyId}", currencyId);
        var currency = await currencyRepository.GetByIdAsync(
            currencyId, disableTracking: true, cancellationToken: cancellationToken);
        if (currency is null)
        {
            logger.Warning("Валюта {CurrencyId} не найдена", currencyId);
            return Result.Fail(errorsFactory.CurrencyNotFound(currencyId));
        }

        if (currency.IsDeleted)
        {
            logger.Warning("Валюта {CurrencyId} помечена как удаленная", currencyId);
            return Result.Fail(errorsFactory.CurrencyIsSoftDeleted(currencyId));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Проверяет существование банка
    /// </summary>
    /// <param name="bankId">Идентификатор банка</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки</returns>
    private async Task<Result> CheckBankAsync(Guid bankId, CancellationToken cancellationToken)
    {
        logger.Debug("Проверка существования банка: {BankId}", bankId);

        var bank = await bankRepository.GetByIdAsync(
            bankId, disableTracking: true, cancellationToken: cancellationToken);
        if (bank is null)
        {
            logger.Warning("Банк {BankId} не найден", bankId);
            return Result.Fail(errorsFactory.BankNotFound(bankId));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Снимает признак "по умолчанию" с предыдущего счета по умолчанию пользователя, если он был установлен
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    private async Task UnsetDefaultAccountIfExistsAsync(Guid registryHolderId, CancellationToken cancellationToken)
    {
        var previousDefaultAccount =
            await accountRepository.GetDefaultAccountAsync(registryHolderId, cancellationToken);
        if (previousDefaultAccount is not null)
        {
            previousDefaultAccount.UnsetAsDefault();
            logger.Debug("Снят флаг по умолчанию со счета: {AccountId}", previousDefaultAccount.Id);
        }
    }
}