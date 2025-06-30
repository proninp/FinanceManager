using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Contracts.DTOs.AccountTypes;

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
/// Методы-расширения для преобразования сущности TransactionsAccountType в AccountTypeDto
/// </summary>
public static class AccountTypeDtoExtensions
{
    /// <summary>
    /// Преобразует сущность TransactionsAccountType в DTO AccountTypeDto
    /// </summary>
    /// <param name="accountType">Сущность типа банковского счета</param>
    /// <returns>Экземпляр AccountTypeDto</returns>
    public static AccountTypeDto ToDto(this TransactionsAccountType accountType)
    {
        return new AccountTypeDto(
            accountType.Id,
            accountType.Code,
            accountType.Description
        );
    }

    /// <summary>
    /// Преобразует коллекцию сущностей TransactionsAccountType в коллекцию DTO AccountTypeDto
    /// </summary>
    /// <param name="accountTypes">Коллекция сущностей типов банковских счетов</param>
    /// <returns>Коллекция AccountTypeDto</returns>
    public static ICollection<AccountTypeDto> ToDto(this IEnumerable<TransactionsAccountType> accountTypes)
    {
        return accountTypes.Select(ToDto).ToList();
    }
}