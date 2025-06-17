using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности Currency
/// </summary>
public class CurrencyErrorsFactory(IErrorsFactory errorsFactory) : ICurrencyErrorsFactory
{
    private const string EntityName = "Currency";
    private const string CharCodeField = "CharCode";
    private const string NumCodeField = "NumCode";
    
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что валюта с указанным идентификатором не найдена
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NotFound(Guid id) =>
        errorsFactory.NotFound("CURRENCY_NOT_FOUND", EntityName, id);
    
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что валюта с указанным символьным кодом уже существует
    /// </summary>
    /// <param name="charCode">Код валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CharCodeAlreadyExists(string charCode) =>
        errorsFactory.AlreadyExists("CURRENCY_CHARCODE_EXISTS", EntityName, CharCodeField, charCode);

    /// <summary>
    /// Создаёт ошибку, указывающую на то, что валюта с указанным числовым кодом уже существует
    /// </summary>
    /// <param name="numCode">Числовой код валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NumCodeAlreadyExists(string numCode) =>
        errorsFactory.AlreadyExists("CURRENCY_NUMCODE_EXISTS", EntityName, NumCodeField, numCode);

    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения символьного кода валюты
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError CharCodeIsRequired() =>
        errorsFactory.Required("CURRENCY_CHARCODE_REQUIRED", EntityName, CharCodeField);

    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения числового кода валюты
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError NumCodeIsRequired() =>
        errorsFactory.Required("CURRENCY_NUMCODE_REQUIRED", EntityName, NumCodeField);

    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения наименования валюты
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError NameIsRequired() =>
        errorsFactory.Required("CURRENCY_NAME_REQUIRED", EntityName, "Name");

    /// <summary>
    /// Создаёт ошибку, указывающую на невозможность удаления валюты, если она используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteUsedCurrency(Guid id) =>
        errorsFactory.CannotDeleteUsedEntity("CURRENCY_IN_USE", EntityName, id);
}