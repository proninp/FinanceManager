using FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;
using FluentResults;

namespace FinanceManager.TransactionsService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы с участниками системы (владельцами транзакций)
/// </summary>
public interface ITransactionHolderService
{
    /// <summary>
    /// Получает участника системы по его идентификатору
    /// </summary>
    Task<Result<TransactionHolderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает участника системы по идентификатору Telegram
    /// </summary>
    Task<Result<TransactionHolderDto>> GetByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, является ли пользователь администратором
    /// </summary>
    Task<Result<bool>> IsAdminAsync(Guid holderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список участников с фильтрацией
    /// </summary>
    Task<Result<ICollection<TransactionHolderDto>>> GetPagedAsync(
        TransactionHolderFilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет данные участника (например, при синхронизации с внешним сервисом)
    /// </summary>
    Task<Result<TransactionHolderDto>> UpdateAsync(
        UpdateTransactionHolderDto updateDto,
        CancellationToken cancellationToken = default);
}