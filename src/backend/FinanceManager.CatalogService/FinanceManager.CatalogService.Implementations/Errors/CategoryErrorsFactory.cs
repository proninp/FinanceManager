using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности Category (категория доходов/расходов)
/// Предоставляет методы для генерации типовых ошибок, связанных с категориями
/// </summary>
public class CategoryErrorsFactory(IErrorsFactory errorsFactory, ILogger logger) : ICategoryErrorsFactory
{
    private const string EntityName = "Category";
    private const string NameField = "Name";
    private const string RegistryHolderField = "RegistryHolder";

    /// <summary>
    /// Создаёт ошибку, если категория с указанным идентификатором не найдена
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NotFound(Guid id)
    {
        logger.Warning("Категория не найдена: {CategoryId}", id);
        return errorsFactory.NotFound("CATEGORY_NOT_FOUND", EntityName, id);
    }

    /// <summary>
    /// Создаёт ошибку, если название категории не заполнено
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError NameIsRequired()
    {
        logger.Warning("Название {EntityName} обязательно для заполнения", EntityName);
        return errorsFactory.Required("CATEGORY_NAME_REQUIRED", EntityName, NameField);
    }

    /// <summary>
    /// Создаёт ошибку, указывающую на то, что категория с указанным именем уже существует для пользователя
    /// </summary>
    /// <param name="name">Название категории</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NameAlreadyExistsInScope(string name)
    {
        logger.Warning("Название {EntityName} уже существует: {Name}", EntityName, name);
        return errorsFactory.AlreadyExists("CATEGORY_NAME_ALREADY_EXISTS", EntityName, NameField, name);
    }

    /// <summary>
    /// Создаёт ошибку, если невозможно удалить категорию, так как она используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteUsedCategory(Guid id)
    {
        logger.Warning("Невозможно удалить категорию '{CategoryId}', так как она используется в других сущностях", id);
        return errorsFactory.CannotDeleteUsedEntity("CATEGORY_IN_USE", EntityName, id);
    }

    /// <summary>
    /// Создаёт ошибку, если не найден владелец категории
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError RegistryHolderNotFound(Guid registryHolderId)
    {
        logger.Warning("Владелец справочника не найден для категории: {RegistryHolderId}", registryHolderId);
        return errorsFactory.NotFound("CATEGORY_REGISTRYHOLDER_NOT_FOUND", RegistryHolderField, registryHolderId);
    }

    /// <summary>
    /// Создаёт ошибку, если попытка установить родительскую категорию приводит к циклической зависимости
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <param name="parentId">Идентификатор предполагаемой родительской категории</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError RecursiveParentCategoryRelation(Guid id, Guid parentId)
    {
        logger.Warning("Обнаружена циклическая связь родительской категории: {CategoryId} -> {ParentId}",
            id, parentId);
        return errorsFactory.CustomConflictError(
            "CATEGORY_RECURSIVE_PARENT",
            $"Cannot set category '{id}' as child of '{parentId}' due to recursive relation");
    }
}