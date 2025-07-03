using FinanceManager.TransactionsService.Domain.Entities;

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

public static class CreateAccountTypeDtoExtensions
{
    /// <summary>
    /// Преобразует CreateAccountTypeDto в TransactionsAccountType
    /// </summary>
    public static TransactionsAccountType ToAccountType(this CreateAccountTypeDto dto)
    {
        return new TransactionsAccountType(
            code: dto.Code,
            description: dto.Description
        );
    }
}