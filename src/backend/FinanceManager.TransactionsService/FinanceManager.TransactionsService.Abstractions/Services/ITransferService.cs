using FinanceManager.TransactionsService.Contracts.DTOs.Transfers;
using FluentResults;

namespace FinanceManager.TransactionsService.Abstractions.Services;

public interface ITransferService
{
    /// <summary>
    /// Получает перевод по ID
    /// </summary>
    Task<Result<TransferDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список переводов с пагинацией и фильтрацией
    /// </summary>
    Task<Result<ICollection<TransferDto>>> GetPagedAsync(
        TransferFilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новый перевод
    /// </summary>
    Task<Result<TransferDto>> CreateAsync(
        CreateTransferDto createDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующий перевод
    /// </summary>
    Task<Result<TransferDto>> UpdateAsync(
        UpdateTransferDto updateDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет перевод
    /// </summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает общее количество переводов по фильтру
    /// </summary>
    Task<Result<int>> GetCountAsync(
        TransferFilterDto filter,
        CancellationToken cancellationToken = default);
}