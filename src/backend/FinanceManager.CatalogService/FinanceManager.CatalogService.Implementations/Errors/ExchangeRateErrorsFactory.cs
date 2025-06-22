using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности ExchangeRate (обменный курс).
/// Предоставляет методы для генерации типовых ошибок, связанных с обменными курсами
/// </summary>
public class ExchangeRateErrorsFactory(IErrorsFactory errorsFactory, ILogger logger) : IExchangeRateErrorsFactory
{
    private const string EntityName = "ExchangeRate";
    private const string CurrencyIdField = "CurrencyId";
    private const string RateDateField = "RateDate";
    private const string RateField = "Rate";

    /// <summary>
    /// Создаёт ошибку, если обменный курс с указанным идентификатором не найден
    /// </summary>
    /// <param name="id">Идентификатор курса</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NotFound(Guid id)
    {
        logger.Warning("ExchangeRate not found: {ExchangeRateId}", id);
        return errorsFactory.NotFound("EXCHANGERATE_NOT_FOUND", EntityName, id);
    }

    /// <summary>
    /// Создаёт ошибку, если курс для валюты на указанную дату уже существует
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="rateDate">Дата курса</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError AlreadyExists(Guid currencyId, DateTime rateDate)
    {
        logger.Warning("ExchangeRate already exists for currency {CurrencyId} on {RateDate}", currencyId, rateDate);
        return errorsFactory.AlreadyExists("EXCHANGERATE_EXISTS", EntityName, $"{CurrencyIdField}:{RateDateField}", $"{currencyId}:{rateDate:yyyy-MM-dd}");
    }

    /// <summary>
    /// Создаёт ошибку, если валюта не указана
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError CurrencyIsRequired()
    {
        logger.Warning("{EntityName} currency is required", EntityName);
        return errorsFactory.Required("EXCHANGERATE_CURRENCY_REQUIRED", EntityName, CurrencyIdField);
    }

    /// <summary>
    /// Создаёт ошибку, если дата курса не указана
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError RateDateIsRequired()
    {
        logger.Warning("{EntityName} rate date is required", EntityName);
        return errorsFactory.Required("EXCHANGERATE_RATEDATE_REQUIRED", EntityName, RateDateField);
    }

    /// <summary>
    /// Создаёт ошибку, если значение курса не указано
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError RateValueIsRequired()
    {
        logger.Warning("{EntityName} rate value is required", EntityName);
        return errorsFactory.Required("EXCHANGERATE_VALUE_REQUIRED", EntityName, RateField);
    }

    /// <summary>
    /// Создаёт ошибку, если невозможно удалить курс, так как он используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор курса</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteUsedExchangeRate(Guid id)
    {
        logger.Warning("Cannot delete exchange rate '{ExchangeRateId}' because it is used in other entities", id);
        return errorsFactory.CannotDeleteUsedEntity("EXCHANGERATE_IN_USE", EntityName, id);
    }
}
