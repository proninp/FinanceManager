using System.Net;
using FinanceManager.CatalogService.Implementations.Extensions;
using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors;

public static class ErrorsFactory
{
    public static Error NotFound(string entityName, long id) =>
        Create($"{entityName.FirstCharToUpper()} with id '{id}' was not found.", HttpStatusCode.NotFound, "Not Found");

    public static Error AlreadyExists(string entityName, string propertyName, object value) =>
        Create($"{entityName.FirstCharToUpper()} with {propertyName} '{value}' already exists.",
            HttpStatusCode.Conflict, "Conflict");

    public static Error Required(string entityName, string propertyName) =>
        Create($"{entityName.FirstCharToUpper()} {propertyName.FirstCharToUpper()} can't be empty.",
            HttpStatusCode.BadRequest,
            "Bad Request");

    private static Error Create(string message, HttpStatusCode status, string title) =>
        new Error(message)
            .WithMetadata(nameof(HttpStatusCode), status)
            .WithMetadata("Title", title);
}