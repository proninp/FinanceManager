using FinanceManager.CatalogService.Contracts.DTOs.Accounts;
using FinanceManager.CatalogService.Domain.Entities;
using Mapster;

namespace FinanceManager.CatalogService.Implementations.Mappings;

/// <summary>
/// Конфигурация маппингов для сущности Account
/// </summary>
public static class AccountMappingConfig
{
    /// <summary>
    /// Маппинг Entity → DTO (для возврата данных)
    /// </summary>
    private static void ConfigureEntityToDto()
    {
        TypeAdapterConfig<Account, AccountDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.RegistryHolder, src => src.RegistryHolder)
            .Map(dest => dest.AccountType, src => src.AccountType)
            .Map(dest => dest.Currency, src => src.Currency)
            .Map(dest => dest.Bank, src => src.Bank)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.IsIncludeInBalance, src => src.IsIncludeInBalance)
            .Map(dest => dest.IsDefault, src => src.IsDefault)
            .Map(dest => dest.IsArchived, src => src.IsArchived)
            .Map(dest => dest.CreditLimit, src => src.CreditLimit);
    }
}