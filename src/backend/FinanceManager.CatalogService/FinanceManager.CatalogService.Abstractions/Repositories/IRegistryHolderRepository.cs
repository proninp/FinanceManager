using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;
using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Abstractions.Repositories;

public interface IRegistryHolderRepository : IBaseRepository<RegistryHolder, RegistryHolderFilterDto>
{
    /// <summary>
    /// Проверяет уникальность Telegram ID
    /// </summary>
    /// <param name="telegramId">Идентификатор пользователя в Telegram</param>
    /// <param name="excludeId">Исключить владельца с данным ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если Telegram ID уникален</returns>
    Task<bool> IsTelegramIdUniqueAsync(long telegramId, Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет существование владельца по Telegram ID
    /// </summary>
    /// <param name="telegramId">Идентификатор пользователя в Telegram</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если владелец с таким Telegram ID существует</returns>
    Task<bool> ExistsByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default);
}