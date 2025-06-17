using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors.Abstractions;

/// <summary>
/// Интерфейс фабрики ошибок, связанных с сущностью "Currency"
/// </summary>
public interface ICurrencyErrorsFactory
{
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что валюта с указанным идентификатором не найдена
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NotFound(Guid id);

    /// <summary>
    /// Создаёт ошибку, указывающую на то, что валюта с указанным кодом валюты уже существует
    /// </summary>
    /// <param name="charCode">Код валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CharCodeAlreadyExists(string charCode);
    
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что валюта с указанным числовым кодом уже существует
    /// </summary>
    /// <param name="numCode">Числовой код валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NumCodeAlreadyExists(string numCode);

    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения кода валюты
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError CharCodeIsRequired();
    
    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения числового кода валюты
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError NumCodeIsRequired();
    
    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения наименования валюты
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError NameIsRequired();

    /// <summary>
    /// Создаёт ошибку, указывающую на невозможность удаления валюты, если она используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор валюты</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CannotDeleteUsedCurrency(Guid id);
}