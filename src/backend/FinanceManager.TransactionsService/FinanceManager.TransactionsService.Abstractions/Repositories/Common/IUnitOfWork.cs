namespace FinanceManager.TransactionsService.Abstractions.Repositories.Common;

/// <summary>
/// Интерфейс единицы работы для управления транзакциями и сохранения изменений
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Асинхронно сохраняет все изменения в контексте данных
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество затронутых записей</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken);
}