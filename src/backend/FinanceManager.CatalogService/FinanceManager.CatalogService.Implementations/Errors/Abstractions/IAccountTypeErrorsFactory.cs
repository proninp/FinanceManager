using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors.Abstractions;

/// <summary>
/// Интерфейс фабрики ошибок, связанных с сущностью "AccountType"
/// </summary>
public interface IAccountTypeErrorsFactory
{
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что тип счета с указанным идентификатором не найден
    /// </summary>
    /// <param name="id">Идентификатор типа счета</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NotFound(Guid id);
    
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что тип счета с указанным кодом уже существует
    /// </summary>
    /// <param name="code">Код типа счета</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CodeAlreadyExists(string code);
    
    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения кода типа счета
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError CodeIsRequired();
    
    /// <summary>
    /// Создаёт ошибку, указывающую на невозможность удаления типа счета, если он используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор типа счета</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CannotDeleteUsedAccountType(Guid id);
}