using FinanceManager.TransactionsService.Contracts.DTOs.TransactionCurrencies;
using FluentResults;

namespace FinanceManager.TransactionsService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы с валютами
/// </summary>
public interface ITransactionsCurrencyService
{
    /// <summary>
    /// Получает валюту по идентификатору
    /// </summary>
    Task<Result<TransactionCurrencyDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает валюту по буквенному коду (например, USD)
    /// </summary>
    Task<Result<TransactionCurrencyDto>> GetByCharCodeAsync(string charCode, CancellationToken ct = default);

    /// <summary>
    /// Получает валюту по цифровому коду (например, 840 для USD)
    /// </summary>
    Task<Result<TransactionCurrencyDto>> GetByNumCodeAsync(string numCode, CancellationToken ct = default);

    /// <summary>
    /// Получает список валют с фильтрацией и пагинацией
    /// </summary>
    Task<Result<ICollection<TransactionCurrencyDto>>> GetPagedAsync(
        TransactionCurrencyFilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новую валюту
    /// </summary>
    Task<Result<TransactionCurrencyDto>> CreateAsync(
        CreateTransactionCurrencyDto createDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую валюту
    /// </summary>
    Task<Result<TransactionCurrencyDto>> UpdateAsync(
        UpdateTransactionCurrencyDto updateDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет валюту
    /// </summary>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, существует ли валюта с указанным буквенным кодом
    /// </summary>
    Task<Result<bool>> ExistsByCharCodeAsync(string charCode, CancellationToken ct = default);

    /// <summary>
    /// Проверяет, существует ли валюта с указанным цифровым кодом
    /// </summary>
    Task<Result<bool>> ExistsByNumCodeAsync(string numCode, CancellationToken ct = default);
}