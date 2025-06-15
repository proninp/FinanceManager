using System.Net;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для создания стандартных ошибок, связанных с сущностями и их состояниями
/// </summary>
public class ErrorsFactory : IErrorsFactory
{
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что сущность не найдена
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NotFound(string errorCode, string entityName, Guid id) =>
        Create(errorCode, HttpStatusCode.NotFound, $"{entityName} with id '{id}' not found.");

    /// <summary>
    /// Создаёт ошибку, указывающую на то, что сущность с указанным значением свойства уже существует
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="propertyName">Имя свойства</param>
    /// <param name="value">Значение свойства</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError AlreadyExists(string errorCode, string entityName, string propertyName, object value) =>
        Create(errorCode, HttpStatusCode.Conflict,
            $"{entityName} with {propertyName} '{value}' already exists.");

    /// <summary>
    /// Создаёт ошибку, указывающую на обязательное свойство сущности
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="propertyName">Имя обязательного свойства</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError Required(string errorCode, string entityName, string propertyName) =>
        Create(errorCode, HttpStatusCode.BadRequest,
            $"{entityName} {propertyName} can't be empty.");

    /// <summary>
    /// Создаёт ошибку, указывающую на невозможность удаления используемой сущности
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteUsedEntity(string errorCode, string entityName, Guid id) =>
        Create(errorCode, HttpStatusCode.Conflict,
            $"Cannot delete {entityName} '{id}' because it is used in other entities");
    
    /// <summary>
    /// Создаёт экземпляр ошибки с заданным кодом, статусом и сообщением
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="status">HTTP-статус</param>
    /// <param name="message">Сообщение об ошибке</param>
    /// <returns>Экземпляр ошибки</returns>
    private static Error Create(string errorCode, HttpStatusCode status, string message) =>
        new Error(message)
            .WithMetadata(nameof(HttpStatusCode), status)
            .WithMetadata("Code", errorCode);
}