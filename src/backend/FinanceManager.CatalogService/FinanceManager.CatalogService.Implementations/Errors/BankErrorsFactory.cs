using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности Bank
/// </summary>
public class BankErrorsFactory(IErrorsFactory errorsFactory) : IBankErrorsFactory
{
    private const string EntityName = "Bank";

    /// <summary>
    /// Создаёт ошибку, если банк с указанным идентификатором не найден
    /// </summary>
    /// <param name="id">Идентификатор банка</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NotFound(Guid id) =>
        errorsFactory.NotFound("BANK_NOT_FOUND", EntityName, id);

    /// <summary>
    /// Создаёт ошибку, если банк с указанным именем уже существует в указанной стране
    /// </summary>
    /// <param name="name">Имя банка</param>
    /// <param name="countryName">Имя страны</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NameAlreadyExists(string name, string countryName) =>
        errorsFactory.AlreadyExists("BANK_NAME_EXISTS", EntityName, "Name", name);

    /// <summary>
    /// Создаёт ошибку, если имя банка не указано
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError NameIsRequired() =>
        errorsFactory.Required("BANK_NAME_REQUIRED", EntityName, "Name");

    /// <summary>
    /// Создаёт ошибку, если банк не может быть удалён, так как используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор банка</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteUsedBank(Guid id) =>
        errorsFactory.CannotDeleteUsedEntity("BANK_IN_USE", EntityName, id);
}