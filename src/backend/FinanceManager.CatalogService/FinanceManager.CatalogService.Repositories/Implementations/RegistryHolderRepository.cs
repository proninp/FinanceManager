using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.CatalogService.Repositories.Implementations;

/// <summary>
/// Репозиторий для работы с владельцами справочников (RegistryHolder).
/// Предоставляет методы фильтрации, проверки уникальности TelegramId и возможности удаления.
/// </summary>
public class RegistryHolderRepository(DatabaseContext context)
    : BaseRepository<RegistryHolder, RegistryHolderFilterDto>(context), IRegistryHolderRepository
{
    private readonly DatabaseContext _context = context;

    /// <summary>
    /// Применяет фильтры к запросу владельцев справочников.
    /// </summary>
    /// <param name="filter">Фильтр владельцев справочников.</param>
    /// <param name="query">Исходный запрос.</param>
    /// <returns>Запрос с применёнными фильтрами.</returns>
    private protected override IQueryable<RegistryHolder> SetFilters(RegistryHolderFilterDto filter,
        IQueryable<RegistryHolder> query)
    {
        if (filter.TelegramId.HasValue)
        {
            query = query.Where(rh => rh.TelegramId == filter.TelegramId.Value);
        }

        if (filter.Role.HasValue)
        {
            query = query.Where(rh => rh.Role == filter.Role.Value);
        }
        return query;
    }

    /// <summary>
    /// Проверяет уникальность TelegramId среди владельцев справочников.
    /// </summary>
    /// <param name="telegramId">Telegram ID для проверки.</param>
    /// <param name="excludeId">Идентификатор владельца, которого нужно исключить из проверки (опционально).</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если TelegramId уникален, иначе false.</returns>
    public async Task<bool> IsTelegramIdUniqueAsync(long telegramId, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await IsUniqueAsync(Entities.AsQueryable(),
            predicate: rh => rh.TelegramId == telegramId,
            excludeId, cancellationToken);
    }

    /// <summary>
    /// Проверяет, может ли владелец справочника быть удалён (нет связанных категорий и счетов).
    /// </summary>
    /// <param name="id">Идентификатор владельца справочника.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если владелец может быть удалён, иначе false.</returns>
    public async Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (await _context.Categories.AnyAsync(c => c.RegistryHolderId == id, cancellationToken))
            return false;
        return !await _context.Accounts.AnyAsync(a => a.RegistryHolderId == id, cancellationToken);
    }
}
