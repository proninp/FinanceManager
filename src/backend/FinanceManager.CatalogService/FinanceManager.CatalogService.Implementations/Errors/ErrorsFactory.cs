using System.Net;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors;

public class ErrorsFactory : IErrorsFactory
{
    public IError NotFound(string errorCode, string entityName, Guid id) =>
        Create(errorCode, HttpStatusCode.NotFound, $"{entityName} with id '{id}' not found.");

    public IError AlreadyExists(string errorCode, string entityName, string propertyName, object value) =>
        Create(errorCode, HttpStatusCode.Conflict,
            $"{entityName} with {propertyName} '{value}' already exists.");

    public IError Required(string errorCode, string entityName, string propertyName) =>
        Create(errorCode, HttpStatusCode.BadRequest,
            $"{entityName} {propertyName} can't be empty.");

    public IError CannotDeleteUsedEntity(string errorCode, string entityName, Guid id) =>
        Create(errorCode, HttpStatusCode.Conflict,
            $"Cannot delete {entityName} '{id}' because it is used in other entities");
    
    private static Error Create(string errorCode, HttpStatusCode status, string message) =>
        new Error(message)
            .WithMetadata(nameof(HttpStatusCode), status)
            .WithMetadata("Code", errorCode);
}