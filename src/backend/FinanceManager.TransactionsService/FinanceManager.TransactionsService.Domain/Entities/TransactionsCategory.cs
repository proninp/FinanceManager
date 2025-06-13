using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

/// <summary>
/// Представляет категорию транзакции, принадлежащую конкретному пользователю
/// </summary>
/// <param name="holderId">Идентификатор владельца категории</param>
/// <param name="income">Флаг, указывающий, что категория используется для доходов</param>
/// <param name="expense">Флаг, указывающий, что категория используется для расходов</param>
public class TransactionsCategory(Guid holderId,bool income, bool expense):IdentityModel
{
    /// <summary>
    /// Идентификатор владельца категории — пользователя системы
    /// </summary>
    public Guid HolderId { get; set; } = holderId;
    
    /// <summary>
    /// Владелец категории — пользователь или участник, которому принадлежит эта категория
    /// </summary>
    public TransactionHolder Holder { get; set; } = null!;
    
    /// <summary>
    /// Флаг, указывающий, может ли категория использоваться для транзакций типа "доход"
    /// </summary>
    public bool Income { get; set; } = income;
    
    /// <summary>
    /// Флаг, указывающий, может ли категория использоваться для транзакций типа "расход"
    /// </summary>
    public bool Expense { get; set; } = expense;
}