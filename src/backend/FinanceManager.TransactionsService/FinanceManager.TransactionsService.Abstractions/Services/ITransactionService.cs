using FinanceManager.TransactionsService.Contracts.DTOs.Transactions;
using FluentResults;

namespace FinanceManager.TransactionsService.Abstractions.Services;

public interface ITransactionService
{
    /// <summary>
    /// Получает транзакцию по ID
    /// </summary>
    Task<Result<TransactionDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список транзакций с пагинацией и фильтрацией
    /// </summary>
    Task<Result<ICollection<TransactionDto>>> GetPagedAsync(
        TransactionFilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новую транзакцию
    /// </summary>
    Task<Result<TransactionDto>> CreateAsync(
        CreateTransactionDto createDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую транзакцию
    /// </summary>
    Task<Result<TransactionDto>> UpdateAsync(
        UpdateTransactionDto updateDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет транзакцию
    /// </summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает общее количество транзакций по фильтру
    /// </summary>
    Task<Result<int>> GetCountAsync(
        TransactionFilterDto filter,
        CancellationToken cancellationToken = default);
}