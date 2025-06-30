using FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;
using FluentResults;

namespace FinanceManager.TransactionsService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы со счётами пользователей
/// </summary>
public interface ITransactionAccountService
{
    /// <summary>
    /// Получает счёт по идентификатору
    /// </summary>
    Task<Result<TransactionAccountDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список счетов с фильтрацией и пагинацией
    /// </summary>
    Task<Result<ICollection<TransactionAccountDto>>> GetPagedAsync(
        TransactionAccountFilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все счета пользователя
    /// </summary>
    Task<Result<ICollection<TransactionAccountDto>>> GetByHolderIdAsync(
        Guid holderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новый счёт
    /// </summary>
    Task<Result<TransactionAccountDto>> CreateAsync(
        CreateTransactionAccountDto createDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующий счёт
    /// </summary>
    Task<Result<TransactionAccountDto>> UpdateAsync(
        UpdateTransactionAccountDto updateDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет счёт
    /// </summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли счёт указанному пользователю
    /// </summary>
    Task<Result<bool>> BelongsToUserAsync(Guid accountId, Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Проверяет, не превышает ли сумма кредитный лимит счёта
    /// </summary>
    Task<Result<bool>> IsWithinCreditLimitAsync(
        Guid accountId,
        decimal amount,
        CancellationToken ct = default);

    /// <summary>
    /// Получает тип счёта
    /// </summary>
    Task<Result<string>> GetAccountTypeAsync(Guid accountId, CancellationToken ct = default);
}