using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors;

public static class CountryErrorsFactory
{
    private const string EntityName = "Country";
    
    public static IError NotFound(Guid id) =>
        ErrorsFactory.NotFound("COUNTRY_NOT_FOUND", EntityName, id);

    public static IError NameAlreadyExists(string name) =>
        ErrorsFactory.AlreadyExists("COUNTRY_NAME_EXISTS", EntityName, "Name", name);
    
    public static IError NameIsRequired() =>
        ErrorsFactory.Required("COUNTRY_NAME_REQUIRED", EntityName, "Name");
}