using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности AccountType (тип банковского счета)
/// Предоставляет методы для генерации типовых ошибок, связанных с типами счетов
/// </summary>
public class AccountTypeErrorsFactory(IErrorsFactory errorsFactory, ILogger logger) : IAccountTypeErrorsFactory
{
    private const string EntityName = "AccountType";
    private const string CodeField = "Code";

    /// <summary>
    /// Создаёт ошибку, указывающую на то, что тип счета с указанным идентификатором не найден
    /// </summary>
    /// <param name="id">Идентификатор типа счета</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NotFound(Guid id)
    {
        logger.Warning("Тип счета не найден: {AccountTypeId}", id);
        return errorsFactory.NotFound("ACCOUNTTYPE_NOT_FOUND", EntityName, id);
    }

    /// <summary>
    /// Создаёт ошибку, указывающую на то, что тип счета с указанным кодом уже существует
    /// </summary>
    /// <param name="code">Код типа счета</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CodeAlreadyExists(string code)
    {
        logger.Warning("Тип счета с кодом '{Code}' уже существует", code);
        return errorsFactory.AlreadyExists("ACCOUNTTYPE_CODE_EXISTS", EntityName, CodeField, code);
    }

    /// <summary>
    /// Создаёт ошибку, указывающую на обязательность заполнения кода типа счета
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError CodeIsRequired()
    {
        logger.Warning("Код {EntityName} обязателен для заполнения", EntityName);
        return errorsFactory.Required("ACCOUNTTYPE_CODE_REQUIRED", EntityName, CodeField);
    }

    /// <summary>
    /// Создаёт ошибку, указывающую на невозможность удаления типа счета, если он используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор типа счета</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteUsedAccountType(Guid id)
    {
        logger.Warning("Невозможно удалить тип счета '{AccountTypeId}', так как он используется в других сущностях",
            id);
        return errorsFactory.CannotDeleteUsedEntity("ACCOUNTTYPE_IN_USE", EntityName, id);
    }
}