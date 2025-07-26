using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности Account (банковский счет)
/// Предоставляет методы для генерации типовых ошибок, связанных со счетами
/// </summary>
public class AccountErrorsFactory(IErrorsFactory errorsFactory, ILogger logger) : IAccountErrorsFactory
{
    private const string EntityName = "Account";
    private const string NameField = "Name";
    private const string CurrencyField = "Currency";
    private const string AccountTypeField = "AccountType";

    /// <summary>
    /// Создаёт ошибку, если счет с указанным идентификатором не найден
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NotFound(Guid id)
    {
        logger.Warning("Счёт не найден: {AccountId}", id);
        return errorsFactory.NotFound("ACCOUNT_NOT_FOUND", EntityName, id);
    }

    /// <summary>
    /// Создаёт ошибку, если у пользователя отсутствует счет по умолчанию
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError DefaultAccountNotFound(Guid registryHolderId)
    {
        logger.Warning("Счёт по умолчанию не найден для владельца реестра: {RegistryHolderId}", registryHolderId);
        return errorsFactory.CustomNotFound(
            "ACCOUNT_DEFAULT_NOT_FOUND",
            $"Default account not found for registry holder: {registryHolderId}");
    }

    /// <summary>
    /// Создаёт ошибку, если название счета не заполнено
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError NameIsRequired()
    {
        logger.Warning("Название {EntityName} обязательно для заполнения", EntityName);
        return errorsFactory.Required(
            "ACCOUNT_NAME_REQUIRED", EntityName, NameField);
    }

    /// <summary>
    /// Создаёт ошибку, если счет нельзя удалить, так как он используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteUsedAccount(Guid id)
    {
        logger.Warning(
            "Невозможно удалить счёт '{AccountId}', так как он используется в других сущностях", id);
        return errorsFactory.CannotDeleteUsedEntity(
            "ACCOUNT_IN_USE", EntityName, id);
    }

    /// <summary>
    /// Создаёт ошибку, при попытке безопасно удалить счет по умолчанию
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotSoftDeleteDefaultAccount(Guid id)
    {
        logger.Warning("Невозможно выполнить мягкое удаление счёта по умолчанию: {AccountId}", id);
        return errorsFactory.CustomConflictError(
            "ACCOUNT_CANNOT_SOFT_DELETE_DEFAULT",
            $"Cannot perform soft deletion for default account '{id}'");
    }

    /// <summary>
    /// Создаёт ошибку, при попытке удалить счет по умолчанию
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteDefaultAccount(Guid id)
    {
        logger.Warning("Невозможно удалить счёт по умолчанию: {AccountId}", id);
        return errorsFactory.CustomConflictError(
            "ACCOUNT_CANNOT_DELETE_DEFAULT",
            $"Cannot delete default account '{id}'");
    }

    /// <summary>
    /// Создаёт ошибку, если счет не может быть установлен как счет по умолчанию, потому что он архивирован или удалён
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError AccountCannotBeSetAsDefaultIfArchivedOrDeleted(Guid id)
    {
        logger.Warning(
            "Счёт не может быть установлен как счёт по умолчанию, так как он архивирован или удалён: {AccountId}", id);
        return errorsFactory.CustomConflictError(
            "ACCOUNT_CANNOT_BE_DEFAULT_IF_ARCHIVED_OR_DELETED",
            $"Account '{id}' cannot be set as default because it is archived or deleted");
    }

    /// <summary>
    /// Создаёт ошибку, если валюта счета была мягко удалена
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CurrencyIsSoftDeleted(Guid currencyId)
    {
        logger.Warning("Валюта '{CurrencyId}' для счёта помечена как удалённая", currencyId);
        return errorsFactory.NotFound(
            "ACCOUNT_CURRENCY_SOFT_DELETED", CurrencyField, currencyId);
    }

    /// <summary>
    /// Создаёт ошибку, если тип счета был мягко удалён
    /// </summary>
    /// <param name="accountTypeId">Идентификатор типа счета</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError AccountTypeIsSoftDeleted(Guid accountTypeId)
    {
        logger.Warning("Тип счёта '{AccountTypeId}' для счёта помечен как удалённый", accountTypeId);
        return errorsFactory.NotFound(
            "ACCOUNT_ACCOUNTTYPE_SOFT_DELETED", AccountTypeField, accountTypeId);
    }

    /// <summary>
    /// Создаёт ошибку, при попытке архивировать счет по умолчанию
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotArchiveDefaultAccount(Guid id)
    {
        logger.Warning("Невозможно архивировать счёт по умолчанию: {AccountId}", id);
        return errorsFactory.CustomConflictError(
            "ACCOUNT_CANNOT_ARCHIVE_DEFAULT",
            $"Cannot archive default account '{id}'");
    }

    /// <summary>
    /// Создаёт ошибку, если при снятии признака "По умолчанию" не найден новый счет по умолчанию по указанному Id
    /// </summary>
    /// <param name="replacementDefaultAccountId">Идентификатор нового счета по умолчанию</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError ReplacementDefaultAccountNotFound(Guid replacementDefaultAccountId)
    {
        logger.Warning("Замещающий счёт по умолчанию не найден: {ReplacementDefaultAccountId}",
            replacementDefaultAccountId);
        return errorsFactory.NotFound(
            "ACCOUNT_REPLACEMENT_DEFAULT_NOT_FOUND", EntityName, replacementDefaultAccountId);
    }

    /// <summary>
    /// Создаёт ошибку, если не найден владелец счета
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError RegistryHolderNotFound(Guid registryHolderId)
    {
        logger.Warning("Владелец реестра не найден: {RegistryHolderId}", registryHolderId);
        return errorsFactory.NotFound("ACCOUNT_REGISTRYHOLDER_NOT_FOUND", "RegistryHolder", registryHolderId);
    }

    /// <summary>
    /// Создаёт ошибку, если не найден тип счета
    /// </summary>
    /// <param name="accountTypeId">Идентификатор типа счета</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError AccountTypeNotFound(Guid accountTypeId)
    {
        logger.Warning("Тип счёта не найден: {AccountTypeId}", accountTypeId);
        return errorsFactory.NotFound("ACCOUNT_ACCOUNTTYPE_NOT_FOUND", "AccountType", accountTypeId);
    }

    /// <summary>
    /// Создаёт ошибку, если не найдена валюта счета
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CurrencyNotFound(Guid currencyId)
    {
        logger.Warning("Валюта не найдена: {CurrencyId}", currencyId);
        return errorsFactory.NotFound("ACCOUNT_CURRENCY_NOT_FOUND", "Currency", currencyId);
    }

    /// <summary>
    /// Создаёт ошибку, если не найден банк
    /// </summary>
    /// <param name="bankId">Идентификатор банка</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError BankNotFound(Guid bankId)
    {
        logger.Warning("Банк не найден: {BankId}", bankId);
        return errorsFactory.NotFound("ACCOUNT_BANK_NOT_FOUND", "Bank", bankId);
    }

    /// <summary>
    /// Создаёт ошибку, если указанный счет не может быть установлен как счет по умолчанию (например, он архивирован или удалён)
    /// </summary>
    /// <param name="replacementDefaultAccountId">Идентификатор счета, который нельзя сделать счетом по умолчанию</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError ReplacementAccountCannotBeSetAsDefault(Guid replacementDefaultAccountId)
    {
        logger.Warning("Замещающий счёт не может быть установлен по умолчанию: {ReplacementDefaultAccountId}",
            replacementDefaultAccountId);
        return errorsFactory.CustomConflictError(
            "ACCOUNT_REPLACEMENT_CANNOT_BE_DEFAULT",
            $"Account '{replacementDefaultAccountId}' cannot be set as default (archived or deleted)");
    }

    /// <summary>
    /// Создаёт ошибку, если владельцы исходного и нового счета по умолчанию не совпадают
    /// </summary>
    /// <param name="id">Идентификатор исходного счета</param>
    /// <param name="replacementDefaultAccountId">Идентификатор нового счета по умолчанию</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError RegistryHolderDiffersBetweenReplacedDefaultAccounts(Guid id, Guid replacementDefaultAccountId)
    {
        logger.Warning(
            "Владельцы реестра отличаются между заменяемыми счетами по умолчанию: {Id} и {ReplacementDefaultAccountId}", id,
            replacementDefaultAccountId);
        return errorsFactory.CustomConflictError(
            "ACCOUNT_REGISTRYHOLDER_DIFFERS",
            $"Registry holders differ between account '{id}' and replacement default account '{replacementDefaultAccountId}'");
    }
}