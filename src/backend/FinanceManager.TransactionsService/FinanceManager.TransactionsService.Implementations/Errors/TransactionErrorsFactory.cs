using FinanceManager.TransactionsService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.TransactionsService.Implementations.Errors;

/// <summary>
/// Фабрика ошибок для сущности Transaction (транзакция)
/// Предоставляет методы для генерации типовых ошибок, связанных с транзакциями
/// </summary>
public class TransactionErrorsFactory(IErrorsFactory errorsFactory, ILogger logger) : ITransactionErrorsFactory
{
    private const string EntityName = "Transaction";
    private const string AccountField = "Account";
    private const string CategoryField = "Category";
    private const string AmountField = "Amount";

    /// <summary>
    /// Создаёт ошибку, если транзакция с указанным идентификатором не найдена
    /// </summary>
    /// <param name="id">Идентификатор транзакции</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError NotFound(Guid id)
    {
        logger.Warning("Transaction not found: {TransactionId}", id);
        return errorsFactory.NotFound("TRANSACTION_NOT_FOUND", EntityName, id);
    }

    /// <summary>
    /// Создаёт ошибку, если сумма транзакции недействительна (например, равна нулю)
    /// </summary>
    /// <returns>Экземпляр ошибки</returns>
    public IError InvalidAmount()
    {
        logger.Warning("Invalid transaction amount");
        return errorsFactory.Required("TRANSACTION_INVALID_AMOUNT", EntityName, AmountField);
    }

    /// <summary>
    /// Создаёт ошибку, если счёт, связанный с транзакцией, не найден
    /// </summary>
    /// <param name="accountId">Идентификатор счёта</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError AccountNotFound(Guid accountId)
    {
        logger.Warning("Account not found for transaction: {AccountId}", accountId);
        return errorsFactory.NotFound("TRANSACTION_ACCOUNT_NOT_FOUND", AccountField, accountId);
    }

    /// <summary>
    /// Создаёт ошибку, если счёт, связанный с транзакцией, был мягко удалён
    /// </summary>
    /// <param name="accountId">Идентификатор счёта</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError AccountIsSoftDeleted(Guid accountId)
    {
        logger.Warning("Account '{AccountId}' for transaction is soft deleted", accountId);
        return errorsFactory.NotFound("TRANSACTION_ACCOUNT_SOFT_DELETED", AccountField, accountId);
    }

    /// <summary>
    /// Создаёт ошибку, если счёт, связанный с транзакцией, архивирован
    /// </summary>
    /// <param name="accountId">Идентификатор счёта</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError AccountIsArchived(Guid accountId)
    {
        logger.Warning("Account '{AccountId}' for transaction is archived", accountId);
        return errorsFactory.CustomConflictError(
            "TRANSACTION_ACCOUNT_ARCHIVED",
            $"Account '{accountId}' is archived and cannot be used for transactions");
    }

    /// <summary>
    /// Создаёт ошибку, если категория транзакции не найдена
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CategoryNotFound(Guid categoryId)
    {
        logger.Warning("Category not found for transaction: {CategoryId}", categoryId);
        return errorsFactory.NotFound("TRANSACTION_CATEGORY_NOT_FOUND", CategoryField, categoryId);
    }

    /// <summary>
    /// Создаёт ошибку, если категория транзакции была мягко удалена
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError CategoryIsSoftDeleted(Guid categoryId)
    {
        logger.Warning("Category '{CategoryId}' for transaction is soft deleted", categoryId);
        return errorsFactory.NotFound("TRANSACTION_CATEGORY_SOFT_DELETED", CategoryField, categoryId);
    }

    /// <summary>
    /// Создаёт ошибку, если транзакция не принадлежит указанному пользователю
    /// </summary>
    /// <param name="transactionId">Идентификатор транзакции</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <returns>Экземпляр ошибки</returns>
    public IError TransactionNotBelongsToUser(Guid transactionId, Guid userId)
    {
        logger.Warning("Transaction '{TransactionId}' does not belong to user: {UserId}", transactionId, userId);
        return errorsFactory.CustomConflictError(
            "TRANSACTION_NOT_BELONGS_TO_USER",
            $"Transaction '{transactionId}' does not belong to user '{userId}'");
    }
}