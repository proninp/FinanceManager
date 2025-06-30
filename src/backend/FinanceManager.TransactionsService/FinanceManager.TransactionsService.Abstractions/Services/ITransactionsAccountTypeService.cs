using FinanceManager.TransactionsService.Contracts.DTOs.AccountTypes;
using FluentResults;

namespace FinanceManager.TransactionsService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы с типами банковских счетов
/// </summary>
public interface ITransactionsAccountTypeService
{
    /// <summary>
    /// Получает тип счёта по идентификатору
    /// </summary>
    Task<Result<AccountTypeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает тип счёта по его коду
    /// </summary>
    Task<Result<AccountTypeDto>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список типов счетов с фильтрацией и пагинацией
    /// </summary>
    Task<Result<ICollection<AccountTypeDto>>> GetPagedAsync(
        AccountTypeFilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новый тип счёта
    /// </summary>
    Task<Result<AccountTypeDto>> CreateAsync(
        CreateAccountTypeDto createDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующий тип счёта
    /// </summary>
    Task<Result<AccountTypeDto>> UpdateAsync(
        UpdateAccountTypeDto updateDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет тип счёта
    /// </summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, существует ли тип счёта с указанным кодом
    /// </summary>
    Task<Result<bool>> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
}