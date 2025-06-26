using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности RegistryHolder (владелец реестра)
/// Предоставляет методы для генерации типовых ошибок, связанных с владельцем реестра
/// </summary>
public class RegistryHolderErrorsFactory(IErrorsFactory errorsFactory, ILogger logger) : IRegistryHolderErrorsFactory
{
    private const string EntityName = "RegistryHolder";
    private const string TelegramIdField = "TelegramId";

    /// <summary>
    /// Создаёт ошибку, если владелец реестра с указанным идентификатором не найден
    /// </summary>
    /// <param name="id">Идентификатор владельца реестра</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NotFound(Guid id)
    {
        logger.Warning("RegistryHolder not found: {RegistryHolderId}", id);
        return errorsFactory.NotFound("REGISTRYHOLDER_NOT_FOUND", EntityName, id);
    }

    /// <summary>
    /// Создаёт ошибку, если TelegramId не указан для владельца реестра
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError TelegramIdIsRequired()
    {
        logger.Warning("{EntityName} telegram id is required", EntityName);
        return errorsFactory.Required("REGISTRYHOLDER_TELEGRAMID_REQUIRED", EntityName, TelegramIdField);
    }

    /// <summary>
    /// Создаёт ошибку, если владелец реестра с указанным TelegramId уже существует
    /// </summary>
    /// <param name="telegramId">TelegramId владельца</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError TelegramIdAlreadyExists(long telegramId)
    {
        logger.Warning("{EntityName} telegram id already exists", EntityName);
        return errorsFactory.AlreadyExists("REGISTRYHOLDER_TELEGRAMID_EXISTS", EntityName, TelegramIdField, telegramId);
    }

    /// <summary>
    /// Создаёт ошибку, если невозможно удалить владельца реестра, так как он используется в других сущностях
    /// </summary>
    /// <param name="id">Идентификатор владельца реестра</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CannotDeleteUsedRegistryHolder(Guid id)
    {
        logger.Warning("Cannot delete registry holder '{RegistryHolderId}' because it is used in other entities", id);
        return errorsFactory.CannotDeleteUsedEntity("REGISTRYHOLDER_IN_USE", EntityName, id);
    }
}