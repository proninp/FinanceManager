using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности "Country"
/// </summary>
public class CountryErrorsFactory(IErrorsFactory errorsFactory, ILogger logger) : ICountryErrorsFactory
{
    private const string EntityName = "Country";

    /// <summary>
    /// Создаёт ошибку, если страна с указанным идентификатором не найдена
    /// </summary>
    public IError NotFound(Guid id)
    {
        logger.Warning("Страна не найдена: {CountryId}", id);
        return errorsFactory.NotFound("COUNTRY_NOT_FOUND", EntityName, id);
    }

    /// <summary>
    /// Создаёт ошибку, если страна с указанным именем уже существует
    /// </summary>
    public IError NameAlreadyExists(string name)
    {
        logger.Warning("Название страны уже существует: {Name}", name);
        return errorsFactory.AlreadyExists("COUNTRY_NAME_EXISTS", EntityName, "Name", name);
    }

    /// <summary>
    /// Создаёт ошибку, если имя страны не указано
    /// </summary>
    public IError NameIsRequired()
    {
        logger.Warning("Название {EntityName} обязательно для заполнения", EntityName);
        return errorsFactory.Required("COUNTRY_NAME_REQUIRED", EntityName, "Name");
    }
    
    /// <summary>
    /// Создаёт ошибку, указывающую на невозможность удаления страны, если она используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор страны</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteUsedCountry(Guid id)
    {
        logger.Warning("Невозможно удалить страну '{CountryId}', так как она используется в других сущностях", id);
        return errorsFactory.CannotDeleteUsedEntity("COUNTRY_IN_USE", EntityName, id);
    }
}