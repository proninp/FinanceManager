using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors.Abstractions;

/// <summary>
/// Интерфейс фабрики ошибок, связанных с сущностью "Bank"
/// </summary>
public interface IBankErrorsFactory
{
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что банк с указанным идентификатором не найден
    /// </summary>
    /// <param name="id">Идентификатор банка</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NotFound(Guid id);
    
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что банк с указанным именем уже существует в указанной стране
    /// </summary>
    /// <param name="name">Имя банка</param>
    /// <param name="countryName">Имя страны</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NameAlreadyExists(string name, string countryName);
    
    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения имени банка
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError NameIsRequired();
    
    /// <summary>
    /// Создаёт ошибку, указывающую на невозможность удаления банка, если он используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор банка</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CannotDeleteUsedBank(Guid id);
}