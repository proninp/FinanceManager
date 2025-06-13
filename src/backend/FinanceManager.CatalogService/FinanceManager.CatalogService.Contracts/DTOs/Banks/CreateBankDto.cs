using FinanceManager.CatalogService.Domain.Entities;

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

/// <summary>
/// Методы-расширения для преобразования CreateBankDto в Bank
/// </summary>
public static class CreateBankDtoExtensions
{
    /// <summary>
    /// Преобразует DTO создания банка в сущность Bank
    /// </summary>
    /// <param name="dto">DTO для создания банка</param>
    /// <returns>Экземпляр Bank</returns>
    public static Bank ToBank(this CreateBankDto dto) =>
        new Bank(dto.CountryId, dto.Name);
}