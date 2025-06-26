using FinanceManager.CatalogService.Contracts.DTOs.Countries;
using FinanceManager.CatalogService.Domain.Entities;

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

/// <summary>
/// Методы-расширения для преобразования сущности Bank в BankDto
/// </summary>
public static class BankDtoExtensions
{
    /// <summary>
    /// Преобразует сущность Bank в DTO BankDto
    /// </summary>
    /// <param name="bank">Сущность банка</param>
    /// <returns>Экземпляр BankDto</returns>
    public static BankDto ToDto(this Bank bank) =>
        new BankDto(bank.Id, bank.Country.ToDto(), bank.Name);

    /// <summary>
    /// Преобразует коллекцию Bank в коллекцию BankDto
    /// </summary>
    /// <param name="banks">Коллекция сущностей банков</param>
    /// <returns>Коллекция BankDto</returns>
    public static ICollection<BankDto> ToDto(this IEnumerable<Bank> banks)
    {
        var dtos = banks.Select(ToDto);
        return dtos as ICollection<BankDto> ?? dtos.ToList();
    }
}