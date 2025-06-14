using System.Net;
using FinanceManager.CatalogService.Implementations.Extensions;
using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика для создания стандартных ошибок приложения с кодами и статусами HTTP
/// </summary>
public static class ErrorsFactory
{
    /// <summary>
    /// Создает ошибку "Не найдено" для указанной сущности и идентификатора
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>Экземпляр ошибки IError</returns>
    public static IError NotFound(string errorCode, string entityName, Guid id) =>
        Create(errorCode, HttpStatusCode.NotFound,
            $"{entityName} with id '{id}' not found.");

    /// <summary>
    /// Создает ошибку "Уже существует" для указанного свойства сущности
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="propertyName">Имя свойства</param>
    /// <param name="value">Значение свойства</param>
    /// <returns>Экземпляр ошибки IError</returns>
    public static IError AlreadyExists(string errorCode, string entityName, string propertyName, object value) =>
        Create(errorCode, HttpStatusCode.Conflict, 
            $"{entityName} with {propertyName} '{value}' already exists.");

    /// <summary>
    /// Создает ошибку обязательного заполнения для указанного свойства сущности
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="propertyName">Имя обязательного свойства</param>
    /// <returns>Экземпляр ошибки IError</returns>
    public static IError Required(string errorCode, string entityName, string propertyName) =>
        Create(errorCode, HttpStatusCode.BadRequest,
            $"{entityName} {propertyName.FirstCharToUpper()} can't be empty.");

    /// <summary>
    /// Создает ошибку невозможности удаления используемой сущности
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>Экземпляр ошибки IError</returns>
    public static IError CannotDeleteUsedEntity(string errorCode, string entityName, Guid id) =>
        Create(errorCode, HttpStatusCode.Conflict,
            $"Cannot delete {entityName} '{id}' because it is used in other entities");

    /// <summary>
    /// Вспомогательный метод для создания экземпляра ошибки с метаданными
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="status">HTTP-статус</param>
    /// <param name="message">Сообщение об ошибке</param>
    /// <returns>Экземпляр ошибки IError</returns>
    private static Error Create(string errorCode, HttpStatusCode status, string message) =>
        new Error(message)
            .WithMetadata(nameof(HttpStatusCode), status)
            .WithMetadata("Code", errorCode);
}