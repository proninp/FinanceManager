using FinanceManager.TransactionsService.Abstractions.Repositories.Common;
using FinanceManager.TransactionsService.Contracts.DTOs.Abstractions;
using FinanceManager.TransactionsService.Contracts.DTOs.Transactions;
using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Abstractions.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с транзакциями
/// </summary>
public interface ITransactionRepository: IBaseRepository<Transaction, TransactionFilterDto>
{
    /// <summary>
    /// Получает список транзакций по владельцу
    /// </summary>
    Task<ICollection<Transaction>> GetByHolderIdAsync(
        Guid holderId,
        bool includeRelated = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли транзакция указанному пользователю
    /// </summary>
    Task<bool> BelongsToUserAsync(Guid transactionId, Guid userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает общее количество транзакций по фильтру
    /// </summary>
    Task<int> GetCountAsync(TransactionFilterDto filter, CancellationToken cancellationToken = default);
}