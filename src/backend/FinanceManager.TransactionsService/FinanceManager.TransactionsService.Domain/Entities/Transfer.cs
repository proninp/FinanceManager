using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

/// <summary>
/// Представляет перевод денежных средств между двумя счетами
/// </summary>
/// <param name="fromAccountId">Идентификатор счёта, с которого осуществляется перевод</param>
/// <param name="toAccountId">Идентификатор счёта, на который осуществляется перевод</param>
/// <param name="fromAmount">Сумма перевода со счёта отправителя</param>
/// <param name="toAmount">Сумма перевода на счёт получателя</param>
/// <param name="description">Необязательное описание перевода</param>
public class Transfer(
    Guid fromAccountId,
    Guid toAccountId,
    decimal fromAmount,
    decimal toAmount,
    string? description = null) : IdentityModel
{
    /// <summary>
    /// Идентификатор счёта, с которого осуществляется перевод
    /// </summary>
    public Guid FromAccountId { get; set; } = fromAccountId;
    
    /// <summary>
    /// Счёт, с которого производится перевод
    /// </summary>
    public TransactionsAccount FromAccount { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор счёта, на который зачисляются средства
    /// </summary>
    public Guid ToAccountId { get; set; } = toAccountId;
    
    /// <summary>
    /// Счёт, на который производится перевод
    /// </summary>
    public TransactionsAccount ToAccount { get; set; } = null!;
    
    /// <summary>
    /// Сумма перевода со счёта отправителя (может включать комиссию)
    /// </summary>
    public decimal FromAmount { get; set; } = fromAmount;
    
    /// <summary>
    /// Сумма перевода, зачисляемая на счёт получателя
    /// </summary>
    public decimal ToAmount { get; set; } = toAmount;
    
    /// <summary>
    /// Описание перевода (необязательное поле)
    /// </summary>
    public string? Description { get; set; } = description;
}