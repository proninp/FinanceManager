namespace FinanceManager.CatalogService.Contracts.DTOs.Banks;

/// <summary>
/// DTO для обновления банка
/// </summary>
/// <param name="Id">Идентификатор банка</param>
/// <param name="CountryId">Идентификатор страны</param>
/// <param name="Name">Название банка</param>
public record UpdateBankDto(
    Guid Id,
    Guid? CountryId = null,
    string? Name = null
);