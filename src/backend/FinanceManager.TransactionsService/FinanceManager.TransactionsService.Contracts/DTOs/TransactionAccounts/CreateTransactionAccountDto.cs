using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;

/// <summary>
/// DTO для создания банковского счета
/// </summary>
/// <param name="TransactionHolderId">Идентификатор владельца реестра</param>
/// <param name="AccountTypeId">Идентификатор типа счета</param>
/// <param name="CurrencyId">Идентификатор валюты счета</param>
/// <param name="CreditLimit">Кредитный лимит по счету</param>
public record CreateTransactionAccountDto(
    Guid TransactionHolderId,
    Guid AccountTypeId,
    Guid CurrencyId,
    decimal? CreditLimit = null
);

public static class CreateTransactionAccountDtoExtensions
{
    /// <summary>
    /// Преобразует CreateTransactionAccountDto в TransactionsAccount
    /// </summary>
    public static TransactionsAccount ToTransactionAccount(this CreateTransactionAccountDto dto)
    {
        return new TransactionsAccount(
            accountTypeId: dto.AccountTypeId,
            currencyId: dto.CurrencyId,
            holderId: dto.TransactionHolderId,
            creditLimit: dto.CreditLimit
        );
    }
}