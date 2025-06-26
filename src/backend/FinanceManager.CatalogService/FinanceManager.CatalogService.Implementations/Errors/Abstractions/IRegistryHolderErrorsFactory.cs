using FluentResults;

namespace FinanceManager.CatalogService.Implementations.Errors.Abstractions;

/// <summary>
/// Фабрика ошибок для сущностей, связанных с владельцем справочника (например, страна, валюта)
/// </summary>
public interface IRegistryHolderErrorsFactory
{
    /// <summary>
    /// Ошибка: владелец справочника не найден
    /// </summary>
    /// <param name="id">Идентификатор владельца</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NotFound(Guid id);

    /// <summary>
    /// Ошибка: TelegramId обязателен для владельца реестра
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError TelegramIdIsRequired();

    /// <summary>
    /// Создаёт ошибку, указывающую на то, что владелец реестра с указанным Telegram Id уже существует
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError TelegramIdAlreadyExists(long telegramId);

    /// <summary>
    /// Ошибка: невозможно удалить владельца, так как он используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор владельца</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CannotDeleteUsedRegistryHolder(Guid id);
}