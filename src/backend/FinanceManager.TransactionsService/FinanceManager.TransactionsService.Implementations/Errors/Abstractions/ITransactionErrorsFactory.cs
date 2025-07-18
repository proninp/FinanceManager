using FluentResults;

namespace FinanceManager.TransactionsService.Implementations.Errors.Abstractions;

/// <summary>
/// Интерфейс фабрики ошибок, связанных с сущностью "Transaction"
/// </summary>
public interface ITransactionErrorsFactory
{
    /// <summary>
    /// Создаёт ошибку, если транзакция с указанным идентификатором не найдена
    /// </summary>
    /// <param name="id">Идентификатор транзакции</param>
    /// <returns>Экземпляр ошибки</returns>
    IError NotFound(Guid id);

    /// <summary>
    /// Создаёт ошибку, если сумма транзакции недействительна (например, равна нулю)
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    IError InvalidAmount();

    /// <summary>
    /// Создаёт ошибку, если счёт, связанный с транзакцией, не найден
    /// </summary>
    /// <param name="accountId">Идентификатор счёта</param>
    /// <returns>Экземпляр ошибки</returns>
    IError AccountNotFound(Guid accountId);

    /// <summary>
    /// Создаёт ошибку, если счёт, связанный с транзакцией, был мягко удалён
    /// </summary>
    /// <param name="accountId">Идентификатор счёта</param>
    /// <returns>Экземпляр ошибки</returns>
    IError AccountIsSoftDeleted(Guid accountId);

    /// <summary>
    /// Создаёт ошибку, если счёт, связанный с транзакцией, архивирован
    /// </summary>
    /// <param name="accountId">Идентификатор счёта</param>
    /// <returns>Экземпляр ошибки</returns>
    IError AccountIsArchived(Guid accountId);

    /// <summary>
    /// Создаёт ошибку, если категория транзакции не найдена
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CategoryNotFound(Guid categoryId);

    /// <summary>
    /// Создаёт ошибку, если категория транзакции была мягко удалена
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <returns>Экземпляр ошибки</returns>
    IError CategoryIsSoftDeleted(Guid categoryId);

    /// <summary>
    /// Создаёт ошибку, если транзакция не принадлежит указанному пользователю
    /// </summary>
    /// <param name="transactionId">Идентификатор транзакции</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Экземпляр ошибки</returns>
    IError TransactionNotBelongsToUser(Guid transactionId, Guid userId);
}