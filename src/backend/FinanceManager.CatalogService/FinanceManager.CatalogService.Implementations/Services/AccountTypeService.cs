using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.AccountTypes;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Services;

/// <summary>
/// Сервис для управления типами банковских счетов.
/// Предоставляет методы для получения, создания, обновления и удаления типов счетов.
/// </summary>
public class AccountTypeService(
    IUnitOfWork unitOfWork,
    IAccountTypeRepository accountTypeRepository,
    IAccountTypeErrorsFactory errorsFactory,
    ILogger logger) : IAccountTypeService
{
    /// <summary>
    /// Получает тип счета по идентификатору
    /// </summary>
    public async Task<Result<AccountTypeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Getting account type by id: {AccountTypeId}", id);

        var accountType =
            await accountTypeRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (accountType is null)
        {
            return Result.Fail(errorsFactory.NotFound(id));
        }

        logger.Information("Successfully retrieved account type: {AccountTypeId}", id);
        return Result.Ok(accountType.ToDto());
    }

    /// <summary>
    /// Получает список типов счетов с фильтрацией и пагинацией
    /// </summary>
    public async Task<Result<ICollection<AccountTypeDto>>> GetPagedAsync(AccountTypeFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Getting paged account types with filter: {@Filter}", filter);
        var types = await accountTypeRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);
        var typesDto = types.ToDto();
        logger.Information("Successfully retrieved {Count} account types", typesDto.Count);
        return Result.Ok(typesDto);
    }

    /// <summary>
    /// Получает все типы счетов без пагинации
    /// </summary>
    public async Task<Result<ICollection<AccountTypeDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.Information("Getting all account types");
        var types = await accountTypeRepository.GetAllAsync(cancellationToken);
        var typesDto = types.ToDto();
        logger.Information("Successfully retrieved {Count} account types", typesDto.Count());
        return Result.Ok(typesDto);
    }

    /// <summary>
    /// Создает новый тип счета
    /// </summary>
    public async Task<Result<AccountTypeDto>> CreateAsync(CreateAccountTypeDto createDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Creating account type: {@CreateDto}", createDto);

        if (string.IsNullOrWhiteSpace(createDto.Code))
        {
            return Result.Fail(errorsFactory.CodeIsRequired());
        }

        if (!await accountTypeRepository.IsCodeUniqueAsync(createDto.Code, cancellationToken: cancellationToken))
        {
            return Result.Fail(errorsFactory.CodeAlreadyExists(createDto.Code));
        }

        var entity = await accountTypeRepository.AddAsync(createDto.ToAccountType(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Successfully created account type: {AccountTypeId}", entity.Id);
        return Result.Ok(entity.ToDto());
    }

    /// <summary>
    /// Обновляет существующий тип счета
    /// </summary>
    public async Task<Result<AccountTypeDto>> UpdateAsync(UpdateAccountTypeDto updateDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Updating account type: {@UpdateDto}", updateDto);

        var entity = await accountTypeRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return Result.Fail(errorsFactory.NotFound(updateDto.Id));
        }

        var isNeedUpdate = false;

        if (!string.IsNullOrWhiteSpace(updateDto.Code) && entity.Code != updateDto.Code)
        {
            if (!await accountTypeRepository.IsCodeUniqueAsync(updateDto.Code, updateDto.Id, cancellationToken))
            {
                return Result.Fail(errorsFactory.CodeAlreadyExists(updateDto.Code));
            }

            entity.Code = updateDto.Code;
            isNeedUpdate = true;
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Description) && !string.Equals(entity.Description, updateDto.Description))
        {
            entity.Description = updateDto.Description;
            isNeedUpdate = true;
        }

        if (isNeedUpdate)
        {
            accountTypeRepository.Update(entity);
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Successfully updated account type: {AccountTypeId}", updateDto.Id);
        }
        else
        {
            logger.Information("No changes detected for account type: {AccountTypeId}", updateDto.Id);
        }

        return Result.Ok(entity.ToDto());
    }

    /// <summary>
    /// Удаляет тип счета
    /// </summary>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Deleting account type: {AccountTypeId}", id);

        if (!await accountTypeRepository.CanBeDeletedAsync(id, cancellationToken))
        {
            return Result.Fail(errorsFactory.CannotDeleteUsedAccountType(id));
        }

        await accountTypeRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows == 0)
        {
            logger.Warning("No account type was deleted for id: {AccountTypeId}", id);
        }

        return Result.Ok();
    }

    /// <summary>
    /// Проверяет уникальность кода типа счета
    /// </summary>
    public async Task<Result<bool>> IsCodeUniqueAsync(string code, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Checking if account type code is unique: {Code}, excludeId: {ExcludeId}", code, excludeId);
        var isUnique = await accountTypeRepository.IsCodeUniqueAsync(code, excludeId, cancellationToken);
        logger.Information("Is account type {Code} unique: {IsUnique}", code, isUnique);
        return Result.Ok(isUnique);
    }

    /// <summary>
    /// Проверяет существование типа счета по коду
    /// </summary>
    public async Task<Result<bool>> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        logger.Information("Checking if account type exists by code: {Code}", code);
        var exists = await accountTypeRepository.ExistsByCodeAsync(code, false, cancellationToken);
        return Result.Ok(exists);
    }

    /// <summary>
    /// Помечает тип счета как удалённый (мягкое удаление)
    /// </summary>
    /// <param name="id">Идентификатор типа счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Soft deleting account type: {AccountTypeId}", id);

        var entity = await accountTypeRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (entity.IsDeleted)
        {
            logger.Information("Account type already soft deleted: {AccountTypeId}", id);
            return Result.Ok();
        }

        entity.MarkAsDeleted();
        accountTypeRepository.Update(entity);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Successfully soft deleted account type: {AccountTypeId}", id);
        return Result.Ok();
    }

    /// <summary>
    /// Восстанавливает ранее мягко удалённый тип счета
    /// </summary>
    /// <param name="id">Идентификатор типа счета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> RestoreAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Restoring account type: {AccountTypeId}", id);

        var entity = await accountTypeRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (!entity.IsDeleted)
        {
            logger.Information("Account type is not deleted, nothing to restore: {AccountTypeId}", id);
            return Result.Ok();
        }

        entity.Restore();
        accountTypeRepository.Update(entity);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Successfully restored account type: {AccountTypeId}", id);
        return Result.Ok();
    }
}