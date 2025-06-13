using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Contracts.DTOs.AccountTypes;

/// <summary>
/// DTO для создания типа банковского счета
/// </summary>
/// <param name="Code">Код типа счета</param>
/// <param name="Description">Описание типа счета</param>
public record CreateAccountTypeDto(
    string Code,
    string Description
);

/// <summary>
/// Методы-расширения для преобразования CreateAccountTypeDto в AccountType
/// </summary>
public static class CreateAccountTypeDtoExtensions
{
    /// <summary>
    /// Преобразует DTO создания типа счета в сущность AccountType
    /// </summary>
    /// <param name="dto">DTO для создания типа банковского счета</param>
    /// <returns>Экземпляр AccountType</returns>
    public static AccountType ToAccountType(this CreateAccountTypeDto dto) =>
        new AccountType(dto.Code, dto.Description);
}
