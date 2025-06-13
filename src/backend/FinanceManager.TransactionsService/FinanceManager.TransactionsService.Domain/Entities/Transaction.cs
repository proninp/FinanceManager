using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

/// <summary>
/// Представляет транзакцию — запись о доходе или расходе денежных средств
/// </summary>
/// <param name="accountId">Идентификатор счёта, на который была произведена транзакция</param>
/// <param name="categoryId">Идентификатор категории транзакции</param>
/// <param name="amount">Сумма транзакции</param>
/// <param name="description">Необязательное описание транзакции</param>
public class Transaction(Guid accountId, Guid categoryId, decimal amount, string? description = null) : IdentityModel
{
    /// <summary>
    /// Идентификатор счёта, связанного с транзакцией
    /// </summary>
    public Guid AccountId { get; set; } = accountId;
    
    /// <summary>
    /// Счёт, на который относится транзакция
    /// </summary>
    public TransactionsAccount Account { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор категории, к которой относится транзакция
    /// </summary>
    public Guid CategoryId { get; set; } = categoryId;
    
    /// <summary>
    /// Категория транзакции (доход или расход)
    /// </summary>
    public TransactionsCategory Category { get; set; } = null!;
    
    /// <summary>
    /// Сумма транзакции в валюте соответствующего счёта
    /// </summary>
    public decimal Amount { get; set; } = amount;
    
    /// <summary>
    /// Описание транзакции (необязательное поле)
    /// </summary>
    public string? Description { get; set; } = description;
}