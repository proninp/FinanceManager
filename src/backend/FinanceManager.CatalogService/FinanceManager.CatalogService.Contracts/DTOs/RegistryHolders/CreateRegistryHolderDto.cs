using FinanceManager.CatalogService.Domain.Enums;

namespace FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;

/// <summary>
/// DTO для создания владельца реестра
/// </summary>
/// <param name="TelegramId">Идентификатор пользователя в Telegram</param>
/// <param name="Role">Роль пользователя в системе</param>
public record CreateRegistryHolderDto(
    long TelegramId,
    Role Role = Role.User
);