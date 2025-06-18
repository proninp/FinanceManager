namespace FinanceManager.TransactionsService.Contracts.DTOs.AccountTypes;

/// <summary>
/// DTO для создания типа банковского счета
/// </summary>
/// <param name="Code">Код типа счета</param>
/// <param name="Description">Описание типа счета</param>
public record CreateAccountTypeDto(
    string Code,
    string Description
);