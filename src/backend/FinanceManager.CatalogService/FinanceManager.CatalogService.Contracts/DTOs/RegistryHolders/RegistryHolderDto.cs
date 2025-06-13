using FinanceManager.CatalogService.Domain.Entities;
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

/// <summary>
/// Методы-расширения для преобразования сущности RegistryHolder в RegistryHolderDto
/// </summary>
public static class RegistryHolderDtoExtensions
{
    /// <summary>
    /// Преобразует сущность RegistryHolder в DTO RegistryHolderDto
    /// </summary>
    /// <param name="registryHolder">Сущность владельца реестра</param>
    /// <returns>Экземпляр RegistryHolderDto</returns>
    public static RegistryHolderDto ToDto(this RegistryHolder registryHolder) =>
        new RegistryHolderDto(
            registryHolder.Id,
            registryHolder.TelegramId,
            registryHolder.Role);
    
    /// <summary>
    /// Преобразует коллекцию RegistryHolder в коллекцию RegistryHolderDto
    /// </summary>
    /// <param name="registryHolders">Коллекция сущностей владельцев реестра</param>
    /// <returns>Коллекция RegistryHolderDto</returns>
    public static IEnumerable<RegistryHolderDto> ToDto(this IEnumerable<RegistryHolder> registryHolders) =>
        registryHolders.Select(holder => holder.ToDto());
}