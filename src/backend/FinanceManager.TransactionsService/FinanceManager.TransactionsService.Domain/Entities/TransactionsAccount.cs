using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

/// <summary>
/// Представляет банковский или финансовый счёт пользователя в системе управления транзакциями
/// </summary>
/// <param name="accountTypeId">Идентификатор типа счёта</param>
/// <param name="currencyId">Идентификатор валюты счёта</param>
/// <param name="holderId">Идентификатор владельца счёта</param>
/// <param name="isArchived">Архивирован ли счет</param>
/// <param name="creditLimit">Необязательный кредитный лимит по счёту (например, для кредитных карт)</param>

public class TransactionsAccount(Guid accountTypeId, Guid currencyId, Guid holderId, bool isArchived = false, decimal? creditLimit = null)
    : SoftDeletableEntity
{
    /// <summary>
    /// Идентификатор типа счёта (например, расчётный, кредитный и т.д.)
    /// </summary>
    public Guid AccountTypeId { get; set; } = accountTypeId;
    
    /// <summary>
    /// Тип счёта, к которому относится данный счёт
    /// </summary>
    public TransactionsAccountType AccountType { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор валюты, в которой ведётся учёт средств на счёте
    /// </summary>
    public Guid CurrencyId { get; set; } = currencyId;
    
    /// <summary>
    /// Валюта, используемая на данном счёте
    /// </summary>
    public TransactionsCurrency Currency { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор владельца счёта
    /// </summary>
    public Guid HolderId { get; set; } = holderId;
    
    /// <summary>
    /// Владелец счёта — пользователь или участник системы
    /// </summary>
    public TransactionHolder Holder { get; set; } = null!;
    
    /// <summary>
    /// Кредитный лимит по счёту (если применимо, например, для кредитных карт)
    /// </summary>
    public decimal? CreditLimit { get; set; } = creditLimit;
    
    /// <summary>
    /// Флаг архивирования счета
    /// </summary>
    public bool IsArchived { get; set; } = isArchived;
}