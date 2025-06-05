namespace FinanceManager.CatalogService.Contracts.DTOs.Banks;

/// <summary>
/// DTO для создания банка
/// </summary>
/// <param name="CountryId">Идентификатор страны</param>
/// <param name="Name">Название банка</param>
public record CreateBankDto(
    Guid CountryId,
    string Name
);