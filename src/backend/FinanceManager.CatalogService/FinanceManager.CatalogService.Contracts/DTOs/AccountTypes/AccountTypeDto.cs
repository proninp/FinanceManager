using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Contracts.DTOs.AccountTypes;

/// <summary>
/// DTO для типа банковского счета
/// </summary>
/// <param name="Id">Идентификатор типа счета</param>
/// <param name="Code">Код типа счета</param>
/// <param name="Description">Описание типа счета</param>
public record AccountTypeDto(
    Guid Id,
    string Code,
    string Description
);

/// <summary>
/// Методы-расширения для преобразования сущности AccountType в AccountTypeDto
/// </summary>
public static class AccountTypeDtoExtensions
{
    /// <summary>
    /// Преобразует сущность AccountType в DTO AccountTypeDto
    /// </summary>
    /// <param name="accountType">Сущность типа банковского счета</param>
    /// <returns>Экземпляр AccountTypeDto</returns>
    public static AccountTypeDto ToDto(this AccountType accountType) =>
        new AccountTypeDto(accountType.Id, accountType.Code, accountType.Description);
    
    /// <summary>
    /// Преобразует коллекцию AccountType в коллекцию AccountTypeDto
    /// </summary>
    /// <param name="accountTypes">Коллекция сущностей типов банковских счетов</param>
    /// <returns>Коллекция AccountTypeDto</returns>
    public static IEnumerable<AccountTypeDto> ToDto(this IEnumerable<AccountType> accountTypes) =>
        accountTypes.Select(accountType => accountType.ToDto());
}