using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

public class TransactionsAccount(Guid accountTypeId, Guid currencyId, Guid holderId, decimal? creditLimit = null)
    : IdentityModel
{
    public Guid AccountTypeId { get; set; } = accountTypeId;
    public TransactionsAccountType AccountType { get; set; } = null!;
    public Guid CurrencyId { get; set; } = currencyId;
    public TransactionsCurrency Currency { get; set; } = null!;
    public Guid HolderId { get; set; } = holderId;
    public TransactionHolder Holder { get; set; } = null!;
    public decimal? CreditLimit { get; set; } = creditLimit;
}