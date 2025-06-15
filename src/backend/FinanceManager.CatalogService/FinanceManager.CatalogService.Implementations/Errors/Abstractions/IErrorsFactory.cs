using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors.Abstractions;

/// <summary>
/// Интерфейс фабрики ошибок для создания стандартных ошибок, связанных с сущностями и их состояниями
/// </summary>
public interface IErrorsFactory
{
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что сущность не найдена
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NotFound(string errorCode, string entityName, Guid id);
    
    /// <summary>
    /// Создаёт ошибку, указывающую на то, что сущность с указанным значением свойства уже существует
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="propertyName">Имя свойства</param>
    /// <param name="value">Значение свойства</param>
    /// <returns>Экземпляр ошибки</returns>
    IError AlreadyExists(string errorCode, string entityName, string propertyName, object value);
    
    /// <summary>
    /// Создаёт ошибку, указывающую на обязательное свойство сущности
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="propertyName">Имя обязательного свойства</param>
    /// <returns>Экземпляр ошибки</returns>
    IError Required(string errorCode, string entityName, string propertyName);
    
    /// <summary>
    /// Создаёт ошибку, указывающую на невозможность удаления используемой сущности
    /// </summary>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="entityName">Имя сущности</param>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CannotDeleteUsedEntity(string errorCode, string entityName, Guid id);
}