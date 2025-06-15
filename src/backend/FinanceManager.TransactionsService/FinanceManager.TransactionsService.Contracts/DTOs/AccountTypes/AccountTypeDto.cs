namespace FinanceManager.TransactionsService.Contracts.DTOs.AccountTypes;

public record AccountTypeDto(
    Guid Id,
    string Code,
    string Description
    );