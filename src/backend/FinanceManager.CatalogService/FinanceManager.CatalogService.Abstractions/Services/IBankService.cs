using FinanceManager.CatalogService.Contracts.DTOs.Banks;
using FluentResults;

namespace FinanceManager.CatalogService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы с банками
/// </summary>
public interface IBankService
{
    /// <summary>
    /// Получает банк по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор банка</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с данными банка или ошибкой</returns>
    Task<Result<BankDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список банков с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком банков или ошибкой</returns>
    Task<Result<ICollection<BankDto>>> GetPagedAsync(
        BankFilterDto filter, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все банки без пагинации
    /// </summary>
    /// <param name="includeRelated">Включать ли связанные сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком всех банков или ошибкой</returns>
    Task<Result<ICollection<BankDto>>> GetAllAsync(
        bool includeRelated = true, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создает новый банк
    /// </summary>
    /// <param name="createDto">Данные для создания банка</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданным банком или ошибкой</returns>
    Task<Result<BankDto>> CreateAsync(
        CreateBankDto createDto, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующий банк
    /// </summary>
    /// <param name="updateDto">Данные для обновления банка</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленным банком или ошибкой</returns>
    Task<Result<BankDto>> UpdateAsync(
        UpdateBankDto updateDto, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет банк
    /// </summary>
    /// <param name="id">Идентификатор банка</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает количество счетов, использующих данный банк
    /// </summary>
    /// <param name="bankId">Идентификатор банка</param>
    /// <param name="includeArchivedAccounts">Включать ли архивированные счета</param>
    /// <param name="includeDeletedAccounts">Включать ли удаленные счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с количеством счетов</returns>
    Task<Result<int>> GetAccountsCountAsync(
        Guid bankId, 
        bool includeArchivedAccounts = false, 
        bool includeDeletedAccounts = false, 
        CancellationToken cancellationToken = default);
}