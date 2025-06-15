using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности "Country"
/// </summary>
public class CountryErrorsFactory(IErrorsFactory errorsFactory) : ICountryErrorsFactory
{
    private const string EntityName = "Country";
    
    /// <summary>
    /// Создаёт ошибку, если страна с указанным идентификатором не найдена
    /// </summary>
    public IError NotFound(Guid id) =>
        errorsFactory.NotFound("COUNTRY_NOT_FOUND", EntityName, id);

    /// <summary>
    /// Создаёт ошибку, если страна с указанным именем уже существует
    /// </summary>
    public IError NameAlreadyExists(string name) =>
        errorsFactory.AlreadyExists("COUNTRY_NAME_EXISTS", EntityName, "Name", name);
    
    /// <summary>
    /// Создаёт ошибку, если имя страны не указано
    /// </summary>
    public IError NameIsRequired() =>
        errorsFactory.Required("COUNTRY_NAME_REQUIRED", EntityName, "Name");
}