using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors.Abstractions;

/// <summary>
/// Интерфейс фабрики ошибок для сущности Category (категория доходов/расходов)
/// Предоставляет методы для генерации типовых ошибок, связанных с категориями
/// </summary>
public interface ICategoryErrorsFactory
{
    /// <summary>
    /// Создаёт ошибку, если категория с указанным идентификатором не найдена
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NotFound(Guid id);

    /// <summary>
    /// Создаёт ошибку, если название категории не заполнено
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError NameIsRequired();

    /// <summary>
    /// Создаёт ошибку, если невозможно удалить категорию, так как она используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CannotDeleteUsedCategory(Guid id);

    /// <summary>
    /// Создаёт ошибку, если не найден владелец категории
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <returns>Экземпляр ошибки</returns>
    IError RegistryHolderNotFound(Guid registryHolderId);

    /// <summary>
    /// Создаёт ошибку, если попытка установить родительскую категорию приводит к циклической зависимости
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <param name="parentId">Идентификатор предполагаемой родительской категории</param>
    /// <returns>Экземпляр ошибки</returns>
    IError RecursiveParentCategoryRelation(Guid id, Guid parentId);

    /// <summary>
    /// Создаёт ошибку, если категория с таким именем уже существует в рамках владельца и уровня иерархии
    /// </summary>
    /// <param name="name">Название категории</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NameAlreadyExistsInScope(string name);
}
