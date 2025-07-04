using FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;
using FluentResults;

namespace FinanceManager.TransactionsService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы с категориями транзакций
/// </summary>
public interface ITransactionsCategoryService
{
    /// <summary>
    /// Получает категорию по идентификатору
    /// </summary>
    Task<Result<TransactionCategoryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список категорий с фильтрацией и пагинацией
    /// </summary>
    Task<Result<ICollection<TransactionCategoryDto>>> GetPagedAsync(
        TransactionCategoryFilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все категории пользователя
    /// </summary>
    Task<Result<ICollection<TransactionCategoryDto>>> GetByHolderIdAsync(
        Guid holderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новую категорию
    /// </summary>
    Task<Result<TransactionCategoryDto>> CreateAsync(
        CreateTransactionCategoryDto createDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую категорию
    /// </summary>
    Task<Result<TransactionCategoryDto>> UpdateAsync(
        UpdateTransactionCategoryDto updateDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет категорию
    /// </summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, что категория принадлежит указанному пользователю
    /// </summary>
    Task<Result<bool>> BelongsToUserAsync(Guid categoryId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, что категория может использоваться для указанного типа транзакции (доход/расход)
    /// </summary>
    Task<Result<bool>> IsValidForTransactionTypeAsync(Guid categoryId, bool isIncome, CancellationToken cancellationToken = default);

}