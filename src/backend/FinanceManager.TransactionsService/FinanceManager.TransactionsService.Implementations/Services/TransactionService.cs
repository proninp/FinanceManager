using FinanceManager.TransactionsService.Abstractions.Repositories;
using FinanceManager.TransactionsService.Abstractions.Repositories.Common;
using FinanceManager.TransactionsService.Abstractions.Services;
using FinanceManager.TransactionsService.Contracts.DTOs.Transactions;
using FinanceManager.TransactionsService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.TransactionsService.Implementations.Services;

public class TransactionService(
    IUnitOfWork unitOfWork,
    ITransactionRepository transactionRepository,
    ITransactionAccountRepository accountRepository,
    ITransactionCategoryRepository categoryRepository,
    ITransactionErrorsFactory errorsFactory,
    ILogger logger) : ITransactionService
{
    /// <summary>
    /// Получает транзакцию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор транзакции</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>DTO транзакции или ошибка, если не найдена</returns>
    public async Task<Result<TransactionDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Getting transaction by id: {TransactionId}", id);
        var transaction = await transactionRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (transaction is null)
        {
            return Result.Fail(errorsFactory.NotFound(id));
        }

        logger.Information("Successfully retrieved transaction: {TransactionId}", id);
        return Result.Ok(transaction.ToDto());
    }

    /// <summary>
    /// Получает список транзакций с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком транзакций или ошибкой</returns>
    public async Task<Result<ICollection<TransactionDto>>> GetPagedAsync(
        TransactionFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Getting paged transactions with filter: {@Filter}", filter);
        var transactions = await transactionRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);
        var transactionsDto = transactions.ToDto();
        logger.Information("Successfully retrieved {Count} transactions", transactionsDto.Count);
        return Result.Ok(transactionsDto);
    }

    /// <summary>
    /// Получает общее количество транзакций по фильтру
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с количеством транзакций или ошибкой</returns>
    public async Task<Result<int>> GetCountAsync(
        TransactionFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Getting transaction count with filter: {@Filter}", filter);
        var count = await transactionRepository.GetCountAsync(filter, cancellationToken);
        logger.Information("Successfully retrieved transaction count: {Count}", count);
        return Result.Ok(count);
    }

    /// <summary>
    /// Создаёт новую транзакцию
    /// </summary>
    /// <param name="createDto">Данные для создания транзакции</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданной транзакцией или ошибкой</returns>
    public async Task<Result<TransactionDto>> CreateAsync(
        CreateTransactionDto createDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Creating transaction: {@CreateDto}", createDto);

        // TODO: Добавить валидацию createDto с использованием FluentValidation
        if (createDto.Amount == 0)
        {
            return Result.Fail(errorsFactory.InvalidAmount());
        }

        var checkResult = await CheckAccountAsync(createDto.AccountId, cancellationToken);
        if (checkResult.IsFailed)
            return Result.Fail(checkResult.Errors);

        checkResult = await CheckCategoryAsync(createDto.CategoryId, cancellationToken);
        if (checkResult.IsFailed)
            return Result.Fail(checkResult.Errors);

        var transaction = await transactionRepository.AddAsync(createDto.ToTransaction(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        logger.Information("Successfully created transaction: {TransactionId}", transaction.Id);
        return Result.Ok(transaction.ToDto());
    }

    /// <summary>
    /// Обновляет существующую транзакцию
    /// </summary>
    /// <param name="updateDto">Данные для обновления транзакции</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновлённой транзакцией или ошибкой</returns>
    public async Task<Result<TransactionDto>> UpdateAsync(
        UpdateTransactionDto updateDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Updating transaction: {@UpdateDto}", updateDto);

        var transaction = await transactionRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (transaction is null)
        {
            return Result.Fail(errorsFactory.NotFound(updateDto.Id));
        }

        var isNeedUpdate = false;

        if (updateDto.Date is not null && transaction.Date != updateDto.Date.Value)
        {
            transaction.Date = updateDto.Date.Value;
            isNeedUpdate = true;
        }

        if (updateDto.Account is not null && transaction.AccountId != updateDto.Account.Value)
        {
            var checkResult = await CheckAccountAsync(updateDto.Account.Value, cancellationToken);
            if (checkResult.IsFailed)
                return Result.Fail(checkResult.Errors);
            transaction.AccountId = updateDto.Account.Value;
            isNeedUpdate = true;
        }

        if (updateDto.Category is not null && transaction.CategoryId != updateDto.Category.Value)
        {
            var checkResult = await CheckCategoryAsync(updateDto.Category.Value, cancellationToken);
            if (checkResult.IsFailed)
                return Result.Fail(checkResult.Errors);
            transaction.CategoryId = updateDto.Category.Value;
            isNeedUpdate = true;
        }

        if (updateDto.Amount is not null && transaction.Amount != updateDto.Amount.Value)
        {
            // TODO: Добавить валидацию updateDto.Amount != 0 с использованием FluentValidation
            if (updateDto.Amount.Value == 0)
            {
                return Result.Fail(errorsFactory.InvalidAmount());
            }
            transaction.Amount = updateDto.Amount.Value;
            isNeedUpdate = true;
        }

        if (updateDto.Description != transaction.Description)
        {
            transaction.Description = updateDto.Description;
            isNeedUpdate = true;
        }

        if (isNeedUpdate)
        {
            transactionRepository.Update(transaction);
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Successfully updated transaction: {TransactionId}", updateDto.Id);
        }
        else
        {
            logger.Information("No changes detected for transaction: {TransactionId}", updateDto.Id);
        }

        return Result.Ok(transaction.ToDto());
    }

    /// <summary>
    /// Удаляет транзакцию
    /// </summary>
    /// <param name="id">Идентификатор транзакции</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Deleting transaction: {TransactionId}", id);

        var transaction = await transactionRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (transaction is null)
        {
            return Result.Ok();
        }

        await transactionRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        logger.Information("Successfully deleted transaction: {TransactionId}", id);
        return Result.Ok();
    }

    /// <summary>
    /// Проверяет существование и актуальность счёта
    /// </summary>
    /// <param name="accountId">Идентификатор счёта</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки</returns>
    protected async Task<Result> CheckAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(accountId, disableTracking: true, cancellationToken: cancellationToken);
        if (account is null)
        {
            return Result.Fail(errorsFactory.AccountNotFound(accountId));
        }

        if (account.IsDeleted)
        {
            return Result.Fail(errorsFactory.AccountIsSoftDeleted(accountId));
        }

        return account.IsArchived ? Result.Fail(errorsFactory.AccountIsArchived(accountId)) : Result.Ok();
    }

    /// <summary>
    /// Проверяет существование и актуальность категории
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки</returns>
    protected async Task<Result> CheckCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(categoryId, disableTracking: true, cancellationToken: cancellationToken);
        if (category is null)
        {
            return Result.Fail(errorsFactory.CategoryNotFound(categoryId));
        }

        return category.IsDeleted ? Result.Fail(errorsFactory.CategoryIsSoftDeleted(categoryId)) : Result.Ok();
    }
}