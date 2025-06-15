using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors;

public static class BankErrorsFactory
{
    private const string EntityName = "Bank";

    public static IError NotFound(Guid id) =>
        ErrorsFactory.NotFound("BANK_NOT_FOUND", EntityName, id);

    public static IError NameAlreadyExists(string name, string countryName) =>
        ErrorsFactory.AlreadyExists("BANK_NAME_EXISTS",
            $"{EntityName} with Name '{name}' in country '{countryName}' already exists.");

    public static IError CannotDeleteUsedBank(Guid id) =>
        ErrorsFactory.CannotDeleteUsedEntity("BANK_IN_USE", EntityName, id);
}