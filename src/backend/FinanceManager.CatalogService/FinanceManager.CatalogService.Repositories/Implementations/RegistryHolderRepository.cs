using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework;
using FinanceManager.CatalogService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FinanceManager.CatalogService.Repositories.Implementations;

/// <summary>
/// Репозиторий для работы с владельцами справочников (RegistryHolder).
/// Предоставляет методы фильтрации, проверки уникальности TelegramId и возможности удаления.
/// </summary>
public class RegistryHolderRepository(DatabaseContext context, ILogger logger)
    : BaseRepository<RegistryHolder, RegistryHolderFilterDto>(context, logger), IRegistryHolderRepository
{
    private readonly DatabaseContext _context = context;
    private readonly ILogger _logger = logger;

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
        var isUnique = await IsUniqueAsync(Entities.AsQueryable(),
            predicate: rh => rh.TelegramId == telegramId,
            excludeId, cancellationToken);

        _logger.Information("Проверка уникальности Telegram ID: {TelegramId} является {IsUnique}",
            telegramId, isUnique ? "уникальным" : "неуникальным");

        return isUnique;
    }

    /// <summary>
    /// Проверяет, может ли владелец справочника быть удалён (нет связанных категорий и счетов).
    /// </summary>
    /// <param name="id">Идентификатор владельца справочника.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>True, если владелец может быть удалён, иначе false.</returns>
    public async Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var hasCategories = await _context.Categories.AnyAsync(c => c.RegistryHolderId == id, cancellationToken);
        if (hasCategories)
        {
            _logger.Information(
                "Владелец справочника {RegistryHolderId} не может быть удален: имеет связанные категории", id);
            return false;
        }

        var hasAccounts = await _context.Accounts.AnyAsync(a => a.RegistryHolderId == id, cancellationToken);
        if (hasAccounts)
        {
            _logger.Information("Владелец справочника {RegistryHolderId} не может быть удален: имеет связанные счета", id);
            return false;
        }
        
        _logger.Information("Владелец справочника {RegistryHolderId} может быть удален", id);
        return true;
    }
}