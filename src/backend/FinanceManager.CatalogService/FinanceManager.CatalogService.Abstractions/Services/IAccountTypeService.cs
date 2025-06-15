using FinanceManager.CatalogService.Contracts.DTOs.AccountTypes;
using FluentResults;

namespace FinanceManager.CatalogService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы с типами банковских счетов
/// </summary>
public interface IAccountTypeService
{
    /// <summary>
    /// Получает тип счета по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор типа счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с данными типа счета или ошибкой</returns>
    Task<Result<AccountTypeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список типов счетов с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком типов счетов или ошибкой</returns>
    Task<Result<ICollection<AccountTypeDto>>> GetPagedAsync(
        AccountTypeFilterDto filter,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получает все типы счетов без пагинации
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком всех типов счетов или ошибкой</returns>
    Task<Result<IEnumerable<AccountTypeDto>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Создает новый тип счета
    /// </summary>
    /// <param name="createDto">Данные для создания типа счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданным типом счета или ошибкой</returns>
    Task<Result<AccountTypeDto>> CreateAsync(
        CreateAccountTypeDto createDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующий тип счета
    /// </summary>
    /// <param name="updateDto">Данные для обновления типа счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленным типом счета или ошибкой</returns>
    Task<Result<AccountTypeDto>> UpdateAsync(
        UpdateAccountTypeDto updateDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет тип счета
    /// </summary>
    /// <param name="id">Идентификатор типа счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Проверяет уникальность кода типа счета
    /// </summary>
    /// <param name="code">Код типа счета</param>
    /// <param name="excludeId">Исключить тип счета с данным ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки уникальности</returns>
    Task<Result<bool>> IsCodeUniqueAsync(
        string code, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет существование типа счета по коду
    /// </summary>
    /// <param name="code">Код типа счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки существования</returns>
    Task<Result<bool>> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
}