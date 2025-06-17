using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности Bank
/// </summary>
public class BankErrorsFactory(IErrorsFactory errorsFactory, ILogger logger) : IBankErrorsFactory
{
    private const string EntityName = "Bank";

    /// <summary>
    /// Создаёт ошибку, если банк с указанным идентификатором не найден
    /// </summary>
    /// <param name="id">Идентификатор банка</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NotFound(Guid id)
    {
        logger.Warning("Bank not found: {BankId}", id);
        return errorsFactory.NotFound("BANK_NOT_FOUND", EntityName, id);
    }

    /// <summary>
    /// Создаёт ошибку, если банк с указанным именем уже существует в указанной стране
    /// </summary>
    /// <param name="name">Имя банка</param>
    /// <param name="countryId">Id страны</param>
    /// <param name="countryName">Имя страны</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NameAlreadyExists(string name, Guid countryId, string countryName)
    {
        logger.Warning("Bank name already exists: {Name} for country with id '{CountryId}'", name,
            countryId);
        return errorsFactory.AlreadyExists("BANK_NAME_EXISTS", EntityName, "Name", name);
    }

    /// <summary>
    /// Создаёт ошибку, если имя банка не указано
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError NameIsRequired()
    {
        logger.Warning("Bank name is required");
        return errorsFactory.Required("BANK_NAME_REQUIRED", EntityName, "Name");
    }

    /// <summary>
    /// Создаёт ошибку, если банк не может быть удалён, так как используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор банка</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteUsedBank(Guid id)
    {
        logger.Warning("Cannot delete bank '{BankId}' because it is using in other entities", id);
        return errorsFactory.CannotDeleteUsedEntity("BANK_IN_USE", EntityName, id);
    }
}