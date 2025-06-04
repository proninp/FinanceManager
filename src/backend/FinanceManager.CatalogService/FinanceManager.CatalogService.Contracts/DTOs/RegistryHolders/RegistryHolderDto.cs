using FinanceManager.CatalogService.Domain.Enums;

namespace FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;

/// <summary>
/// DTO для владельца реестра финансовых данных
/// </summary>
/// <param name="Id">Уникальный идентификатор владельца</param>
/// <param name="TelegramId">Идентификатор пользователя в Telegram</param>
/// <param name="Role">Роль пользователя в системе</param>
public record RegistryHolderDto(
    Guid Id,
    long TelegramId,
    Role Role
);