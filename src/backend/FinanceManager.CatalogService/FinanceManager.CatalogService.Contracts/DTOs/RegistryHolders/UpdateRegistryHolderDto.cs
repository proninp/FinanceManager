using FinanceManager.CatalogService.Domain.Enums;

namespace FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;

/// <summary>
/// DTO для обновления владельца реестра
/// </summary>
/// <param name="Id">Уникальный идентификатор владельца</param>
/// <param name="TelegramId">Идентификатор пользователя в Telegram</param>
/// <param name="Role">Роль пользователя в системе</param>
public record UpdateRegistryHolderDto(
    Guid Id,
    long? TelegramId,
    Role? Role
);