using FinanceManager.CatalogService.Contracts.DTOs.Countries;

namespace FinanceManager.CatalogService.Contracts.DTOs.Banks;

/// <summary>
/// DTO для банка
/// </summary>
/// <param name="Id">Идентификатор банка</param>
/// <param name="Country">Страна, в которой находится банк</param>
/// <param name="Name">Название банка</param>
public record BankDto(
    Guid Id,
    CountryDto Country,
    string Name
);