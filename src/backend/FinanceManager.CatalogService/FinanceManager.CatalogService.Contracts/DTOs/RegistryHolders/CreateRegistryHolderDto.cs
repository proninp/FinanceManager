using FinanceManager.CatalogService.Domain.Entities;
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

/// <summary>
/// Методы-расширения для преобразования CreateRegistryHolderDto в RegistryHolder
/// </summary>
public static class CreateRegistryHolderDtoExtensions
{
    /// <summary>
    /// Преобразует DTO создания владельца реестра в сущность RegistryHolder
    /// </summary>
    /// <param name="dto">DTO для создания владельца реестра</param>
    /// <returns>Экземпляр RegistryHolder</returns>
    public static RegistryHolder ToRegistryHolder(this CreateRegistryHolderDto dto) =>
        new RegistryHolder(dto.TelegramId, dto.Role);
}