using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors.Abstractions;

/// <summary>
/// Фабрика ошибок для сущности ExchangeRate
/// </summary>
public interface IExchangeRateErrorsFactory
{
    /// <summary>
    /// Создаёт ошибку, если обменный курс с указанным идентификатором не найден
    /// </summary>
    /// <param name="id">Идентификатор курса</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NotFound(Guid id);

    /// <summary>
    /// Создаёт ошибку, если курс для валюты на указанную дату уже существует
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты</param>
    /// <param name="rateDate">Дата курса</param>
    /// <returns>Экземпляр ошибки</returns>
    IError AlreadyExists(Guid currencyId, DateTime rateDate);
    
    /// <summary>
    /// Создаёт ошибку, если валюта не указана
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError CurrencyIsRequired();

    /// <summary>
    /// Создаёт ошибку, если дата курса не указана
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError RateDateIsRequired();

    /// <summary>
    /// Создаёт ошибку, если значение курса не указано
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError RateValueIsRequired();

    /// <summary>
    /// Создаёт ошибку, если невозможно удалить курс, так как он используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор курса</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CannotDeleteUsedExchangeRate(Guid id);
}