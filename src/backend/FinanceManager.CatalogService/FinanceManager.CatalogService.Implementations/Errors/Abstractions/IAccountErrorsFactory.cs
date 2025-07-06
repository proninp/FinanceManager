using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors.Abstractions;

/// <summary>
/// Интерфейс фабрики ошибок, связанных с сущностью "Account"
/// </summary>
public interface IAccountErrorsFactory
{
    /// <summary>
    /// Создаёт ошибку, если счет с указанным идентификатором не найден
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NotFound(Guid id);

    /// <summary>
    /// Создаёт ошибку, если название счета не заполнено
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError NameIsRequired();

    /// <summary>
    /// Создаёт ошибку, если счет нельзя удалить, так как он используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CannotDeleteUsedAccount(Guid id);

    /// <summary>
    /// Создаёт ошибку, если валюта счета была мягко удалена
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CurrencyIsSoftDeleted(Guid currencyId);

    /// <summary>
    /// Создаёт ошибку, если тип счета был мягко удалён
    /// </summary>
    /// <param name="accountTypeId">Идентификатор типа счета</param>
    /// <returns>Экземпляр ошибки</returns>
    IError AccountTypeIsSoftDeleted(Guid accountTypeId);

    /// <summary>
    /// Создаёт ошибку, если нельзя архивировать счет по умолчанию
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CannotArchiveDefaultAccount(Guid id);
    
    /// <summary>
    /// Создаёт ошибку, если счет не может быть установлен как счет по умолчанию, так как он архивирован или удалён
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    IError AccountCannotBeSetAsDefaultIfArchivedOrDeleted(Guid id);

    /// <summary>
    /// Создаёт ошибку, если при снятии признака "По умолчанию" не найден новый счет по умолчанию по указанному Id
    /// </summary>
    /// <param name="replacementDefaultAccountId">Идентификатор нового счета по умолчанию</param>
    /// <returns>Экземпляр ошибки</returns>
    IError ReplacementDefaultAccountNotFound(Guid replacementDefaultAccountId);

    /// <summary>
    /// Создаёт ошибку, если у пользователя отсутствует счет по умолчанию
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <returns>Экземпляр ошибки</returns>
    IError DefaultAccountNotFound(Guid registryHolderId);
    
    /// <summary>
    /// Создаёт ошибку, если не найден владелец счета
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <returns>Экземпляр ошибки</returns>
    IError RegistryHolderNotFound(Guid registryHolderId);
    
    /// <summary>
    /// Создаёт ошибку, если не найден тип счета
    /// </summary>
    /// <param name="registryHolderId">Идентификатор типа счета</param>
    /// <returns>Экземпляр ошибки</returns>
    IError AccountTypeNotFound(Guid registryHolderId);
    
    /// <summary>
    /// Создаёт ошибку, если не найдена валюта счета
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CurrencyNotFound(Guid currencyId);
    
    /// <summary>
    /// Создаёт ошибку, если не найден банк
    /// </summary>
    /// <param name="bankId">Идентификатор банка</param>
    /// <returns>Экземпляр ошибки</returns>
    IError BankNotFound(Guid bankId);

    /// <summary>
    /// Создаёт ошибку, если указанный счет не может быть установлен как счет по умолчанию (например, он архивирован или удалён)
    /// </summary>
    /// <param name="replacementDefaultAccountId">Идентификатор счета, который нельзя сделать счетом по умолчанию</param>
    /// <returns>Экземпляр ошибки</returns>
    IError ReplacementAccountCannotBeSetAsDefault(Guid replacementDefaultAccountId);

    /// <summary>
    /// Создаёт ошибку, если владельцы исходного и нового счета по умолчанию не совпадают
    /// </summary>
    /// <param name="id">Идентификатор исходного счета</param>
    /// <param name="replacementDefaultAccountId">Идентификатор нового счета по умолчанию</param>
    /// <returns>Экземпляр ошибки</returns>
    IError RegistryHolderDiffersBetweenReplacedDefaultAccounts(Guid id, Guid replacementDefaultAccountId);
    
    /// <summary>
    /// Создаёт ошибку, при попытке безопасного удалить счет по умолчанию
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CannotSoftDeleteDefaultAccount(Guid id);

    /// <summary>
    /// Создаёт ошибку, при попытке удалить счет по умолчанию
    /// </summary>
    /// <param name="id">Идентификатор счета</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CannotDeleteDefaultAccount(Guid id);
}