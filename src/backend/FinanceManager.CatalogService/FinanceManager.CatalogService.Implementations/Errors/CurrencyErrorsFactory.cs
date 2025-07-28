using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности Currency
/// </summary>
public class CurrencyErrorsFactory(IErrorsFactory errorsFactory, ILogger logger) : ICurrencyErrorsFactory
{
    private const string EntityName = "Currency";
    private const string CharCodeField = "CharCode";
    private const string NumCodeField = "NumCode";

    /// <summary>
    /// Создаёт ошибку, указывающую на то, что валюта с указанным идентификатором не найдена
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NotFound(Guid id)
    {
        logger.Warning("Валюта не найдена: {CurrencyId}", id);
        return errorsFactory.NotFound("CURRENCY_NOT_FOUND", EntityName, id);
    }

    /// <summary>
    /// Создаёт ошибку, указывающую на то, что валюта с указанным символьным кодом уже существует
    /// </summary>
    /// <param name="charCode">Код валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CharCodeAlreadyExists(string charCode)
    {
        logger.Warning("Буквенный код валюты уже существует: {CharCode}", charCode);
        return errorsFactory.AlreadyExists("CURRENCY_CHARCODE_EXISTS", EntityName, CharCodeField, charCode);
    }

    /// <summary>
    /// Создаёт ошибку, указывающую на то, что валюта с указанным числовым кодом уже существует
    /// </summary>
    /// <param name="numCode">Числовой код валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NumCodeAlreadyExists(string numCode)
    {
        logger.Warning("Числовой код валюты уже существует: {NumCode}", numCode);
        return errorsFactory.AlreadyExists("CURRENCY_NUMCODE_EXISTS", EntityName, NumCodeField, numCode);
    }

    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения символьного кода валюты
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError CharCodeIsRequired()
    {
        logger.Warning("Буквенный код {EntityName} обязателен для заполнения", EntityName);
        return errorsFactory.Required("CURRENCY_CHARCODE_REQUIRED", EntityName, CharCodeField);
    }

    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения числового кода валюты
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError NumCodeIsRequired()
    {
        logger.Warning("Числовой код {EntityName} обязателен для заполнения", EntityName);
        return errorsFactory.Required("CURRENCY_NUMCODE_REQUIRED", EntityName, NumCodeField);
    }

    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения наименования валюты
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError NameIsRequired()
    {
        logger.Warning("Название {EntityName} обязательно для заполнения", EntityName);
        return errorsFactory.Required("CURRENCY_NAME_REQUIRED", EntityName, "Name");
    }

    /// <summary>
    /// Создаёт ошибку, указывающую на невозможность удаления валюты, если она используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteUsedCurrency(Guid id)
    {
        logger.Warning("Невозможно удалить валюту '{CurrencyId}', так как она используется в других сущностях", id);
        return errorsFactory.CannotDeleteUsedEntity("CURRENCY_IN_USE", EntityName, id);
    }
}