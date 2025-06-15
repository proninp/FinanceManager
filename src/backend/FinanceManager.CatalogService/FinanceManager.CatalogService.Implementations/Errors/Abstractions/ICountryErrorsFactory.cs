using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors.Abstractions;

/// <summary>
/// Интерфейс фабрики ошибок, связанных с сущностью "Country"
/// </summary>
public interface ICountryErrorsFactory
{
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что страна с указанным идентификатором не найдена
    /// </summary>
    /// <param name="id">Идентификатор страны</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NotFound(Guid id);
    
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что страна с указанным именем уже существует
    /// </summary>
    /// <param name="name">Имя страны</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NameAlreadyExists(string name);
    
    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения имени страны
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError NameIsRequired();
}