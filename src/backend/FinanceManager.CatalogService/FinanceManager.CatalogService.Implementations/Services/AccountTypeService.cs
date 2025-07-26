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
        logger.Information("Получение типа счета по идентификатору: {AccountTypeId}", id);

        var accountType =
            await accountTypeRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (accountType is null)
        {
            logger.Warning("Тип счета с идентификатором {AccountTypeId} не найден", id);
            return Result.Fail(errorsFactory.NotFound(id));
        }

        logger.Information("Тип счета {AccountTypeId} успешно получен", id);
        return Result.Ok(accountType.ToDto());
    }

    /// <summary>
    /// Получает список типов счетов с фильтрацией и пагинацией
    /// </summary>
    public async Task<Result<ICollection<AccountTypeDto>>> GetPagedAsync(AccountTypeFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Получение списка типов счетов с фильтрацией: {@Filter}", filter);
        var types = await accountTypeRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);
        var typesDto = types.ToDto();
        logger.Information("Получено {Count} типов счетов", typesDto.Count);
        return Result.Ok(typesDto);
    }

    /// <summary>
    /// Получает все типы счетов без пагинации
    /// </summary>
    public async Task<Result<ICollection<AccountTypeDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        logger.Information("Получение всех типов счетов");
        var types = await accountTypeRepository.GetAllAsync(cancellationToken);
        var typesDto = types.ToDto();
        logger.Information("Получено {Count} типов счетов", typesDto.Count);
        return Result.Ok(typesDto);
    }

    /// <summary>
    /// Создает новый тип счета
    /// </summary>
    public async Task<Result<AccountTypeDto>> CreateAsync(CreateAccountTypeDto createDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Создание нового типа счета: {@CreateDto}", createDto);

        if (string.IsNullOrWhiteSpace(createDto.Code))
        {
            logger.Warning("Попытка создания типа счета без указания кода");
            return Result.Fail(errorsFactory.CodeIsRequired());
        }

        if (!await accountTypeRepository.IsCodeUniqueAsync(createDto.Code, cancellationToken: cancellationToken))
        {
            logger.Warning("Попытка создания типа счета с неуникальным кодом: {Code}", createDto.Code);
            return Result.Fail(errorsFactory.CodeAlreadyExists(createDto.Code));
        }

        var entity = await accountTypeRepository.AddAsync(createDto.ToAccountType(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Тип счета {AccountTypeId} с кодом '{Code}' успешно создан", entity.Id, createDto.Code);
        return Result.Ok(entity.ToDto());
    }

    /// <summary>
    /// Обновляет существующий тип счета
    /// </summary>
    public async Task<Result<AccountTypeDto>> UpdateAsync(UpdateAccountTypeDto updateDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Обновление типа счета: {@UpdateDto}", updateDto);

        var entity = await accountTypeRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            logger.Warning("Тип счета с идентификатором {AccountTypeId} не найден для обновления", updateDto.Id);
            return Result.Fail(errorsFactory.NotFound(updateDto.Id));
        }

        var isNeedUpdate = false;

        if (!string.IsNullOrWhiteSpace(updateDto.Code) && entity.Code != updateDto.Code)
        {
            if (!await accountTypeRepository.IsCodeUniqueAsync(updateDto.Code, updateDto.Id, cancellationToken))
            {
                logger.Warning("Попытка обновления типа счета {AccountTypeId} с неуникальным кодом: {Code}",
                    updateDto.Id, updateDto.Code);
                return Result.Fail(errorsFactory.CodeAlreadyExists(updateDto.Code));
            }

            entity.Code = updateDto.Code;
            isNeedUpdate = true;
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Description) &&
            !string.Equals(entity.Description, updateDto.Description))
        {
            entity.Description = updateDto.Description;
            isNeedUpdate = true;
        }

        if (isNeedUpdate)
        {
            // нам не нужно вызывать метод accountTypeRepository.UpdateAsync(), так как сущность accountType уже отслеживается
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Тип счета {AccountTypeId} успешно обновлен", updateDto.Id);
        }
        else
        {
            logger.Information("Изменения для типа счета {AccountTypeId} не обнаружены", updateDto.Id);
        }

        return Result.Ok(entity.ToDto());
    }

    /// <summary>
    /// Удаляет тип счета
    /// </summary>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Жесткое удаление типа счета: {AccountTypeId}", id);

        if (!await accountTypeRepository.CanBeDeletedAsync(id, cancellationToken))
        {
            logger.Warning("Тип счета {AccountTypeId} не может быть удален, так как используется в счетах", id);
            return Result.Fail(errorsFactory.CannotDeleteUsedAccountType(id));
        }

        await accountTypeRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows == 0)
        {
            logger.Warning("Тип счета {AccountTypeId} не был удален, возможно он не существует", id);
        }
        else
        {
            logger.Information("Тип счета {AccountTypeId} успешно удален", id);
        }

        return Result.Ok();
    }

    /// <summary>
    /// Проверяет уникальность кода типа счета
    /// </summary>
    public async Task<Result<bool>> IsCodeUniqueAsync(string code, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        logger.Debug("Проверка уникальности кода типа счета: '{Code}', исключая: {ExcludeId}", code, excludeId);
        var isUnique = await accountTypeRepository.IsCodeUniqueAsync(code, excludeId, cancellationToken);
        logger.Debug("Код типа счета '{Code}' {UniqueResult}", code, isUnique ? "уникален" : "не уникален");
        return Result.Ok(isUnique);
    }

    /// <summary>
    /// Проверяет существование типа счета по коду
    /// </summary>
    public async Task<Result<bool>> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        logger.Debug("Проверка существования типа счета по коду: '{Code}'", code);
        var exists = await accountTypeRepository.ExistsByCodeAsync(code, false, cancellationToken);
        logger.Debug("Тип счета с кодом '{Code}' {ExistsResult}", code, exists ? "существует" : "не существует");
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
        logger.Information("Мягкое удаление типа счета: {AccountTypeId}", id);

        var entity = await accountTypeRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            logger.Warning("Тип счета с идентификатором {AccountTypeId} не найден для мягкого удаления", id);
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (entity.IsDeleted)
        {
            logger.Information("Тип счета {AccountTypeId} уже помечен как удаленный", id);
            return Result.Ok();
        }

        entity.MarkAsDeleted();
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Тип счета {AccountTypeId} успешно помечен как удаленный", id);
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
        logger.Information("Восстановление типа счета: {AccountTypeId}", id);

        var entity = await accountTypeRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            logger.Warning("Тип счета с идентификатором {AccountTypeId} не найден для восстановления", id);
            return Result.Fail(errorsFactory.NotFound(id));
        }

        if (!entity.IsDeleted)
        {
            logger.Information("Тип счета {AccountTypeId} не был удален, восстановление не требуется", id);
            return Result.Ok();
        }

        entity.Restore();
        await unitOfWork.CommitAsync(cancellationToken);
        
        logger.Information("Тип счета {AccountTypeId} успешно восстановлен", id);
        return Result.Ok();
    }
}