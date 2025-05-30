using FinanceManager.CatalogService.Domain.Abstractions;
using FinanceManager.CatalogService.Domain.Enums;

namespace FinanceManager.CatalogService.Domain.Entities;

public class RegistryHolder(long telegramId, Role role) : IdentityModel
{
    public long TelegramId { get; set; } = telegramId;

    public Role Role { get; set; } = role;
}