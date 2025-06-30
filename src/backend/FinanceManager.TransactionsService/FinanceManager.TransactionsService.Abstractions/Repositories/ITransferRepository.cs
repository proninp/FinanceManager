using FinanceManager.TransactionsService.Abstractions.Repositories.Common;
using FinanceManager.TransactionsService.Contracts.DTOs.Transfers;
using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Abstractions.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с переводами
/// </summary>
public interface ITransferRepository: IBaseRepository<Transfer, TransferFilterDto>
{
    /// <summary>
    /// Получает список переводов по владельцу (через счёт)
    /// </summary>
    Task<ICollection<Transfer>> GetByHolderIdAsync(
        Guid holderId,
        bool includeRelated = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли перевод указанному пользователю
    /// </summary>
    Task<bool> BelongsToUserAsync(Guid transferId, Guid userId, CancellationToken cancellationToken = default);
}