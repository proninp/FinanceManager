using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Services;

/// <summary>
/// Сервис для управления владельцами реестра
/// Предоставляет методы для получения, создания, обновления и удаления владельцев реестра
/// </summary>
public class RegistryHolderService(
    IUnitOfWork unitOfWork,
    IRegistryHolderRepository registryHolderRepository,
    ILogger logger,
    IRegistryHolderErrorsFactory registryHolderErrorsFactory
) : IRegistryHolderService
{
    /// <summary>
    /// Получает владельца реестра по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор владельца реестра</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с данными владельца реестра или ошибкой</returns>
    public async Task<Result<RegistryHolderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Getting registry holder by id: {RegistryHolderId}", id);

        var holder =
            await registryHolderRepository.GetByIdAsync(id, disableTracking: true,
                cancellationToken: cancellationToken);
        if (holder is null)
        {
            return Result.Fail(registryHolderErrorsFactory.NotFound(id));
        }

        logger.Information("Successfully retrieved registry holder: {RegistryHolderId}", id);
        return Result.Ok(holder.ToDto());
    }

    /// <summary>
    /// Получает список владельцев реестра с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком владельцев реестра или ошибкой</returns>
    public async Task<Result<ICollection<RegistryHolderDto>>> GetPagedAsync(RegistryHolderFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Getting paged regestry holders with filter: {@Filter}", filter);
        var holders = await registryHolderRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);

        var holdersDtos = holders.ToDto();

        logger.Information("Successfully retrieved {Count} registry holders", holdersDtos.Count);
        return Result.Ok(holdersDtos);
    }

    /// <summary>
    /// Создаёт нового владельца реестра
    /// </summary>
    /// <param name="createDto">Данные для создания владельца реестра</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданным владельцем реестра или ошибкой</returns>
    public async Task<Result<RegistryHolderDto>> CreateAsync(CreateRegistryHolderDto createDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Creating registry holder: {@CreateDto}", createDto);

        if (createDto.TelegramId == 0)
        {
            return Result.Fail(registryHolderErrorsFactory.TelegramIdIsRequired());
        }

        if (!await registryHolderRepository.IsTelegramIdUniqueAsync(createDto.TelegramId, cancellationToken: cancellationToken))
        {
            return Result.Fail(registryHolderErrorsFactory.TelegramIdAlreadyExists(createDto.TelegramId));
        }

        var holder = await registryHolderRepository.AddAsync(createDto.ToRegistryHolder(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Successfully created registry holder: {RegistryHolderId} with telegram Id: {TelegramId}",
            holder.Id, holder.TelegramId);

        return Result.Ok(holder.ToDto());
    }

    /// <summary>
    /// Обновляет существующего владельца реестра
    /// </summary>
    /// <param name="updateDto">Данные для обновления владельца реестра</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленным владельцем реестра или ошибкой</returns>
    public async Task<Result<RegistryHolderDto>> UpdateAsync(UpdateRegistryHolderDto updateDto, CancellationToken cancellationToken = default)
    {
        logger.Information("Updating registry holder: {@UpdateDto}", updateDto);

        var holder = await registryHolderRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (holder is null)
        {
            return Result.Fail(registryHolderErrorsFactory.NotFound(updateDto.Id));
        }

        var isNeedUpdate = false;

        if (updateDto.TelegramId.HasValue && holder.TelegramId != updateDto.TelegramId.Value)
        {
            if (!await registryHolderRepository.IsTelegramIdUniqueAsync(updateDto.TelegramId.Value, updateDto.Id, cancellationToken))
            {
                return Result.Fail(registryHolderErrorsFactory.TelegramIdAlreadyExists(updateDto.TelegramId.Value));
            }
            holder.TelegramId = updateDto.TelegramId.Value;
            isNeedUpdate = true;
        }

        if (updateDto.Role is not null && updateDto.Role.Value != holder.Role)
        {
            holder.Role = updateDto.Role.Value;
            isNeedUpdate = true;
        }

        if (isNeedUpdate)
        {
            registryHolderRepository.Update(holder);
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Successfully updated registry holder: {RegistryHolderId}", updateDto.Id);
        }
        else
        {
            logger.Information("No changes detected for registry holder: {RegistryHolderId}", updateDto.Id);
        }

        return Result.Ok(holder.ToDto());
    }

    /// <summary>
    /// Удаляет владельца реестра
    /// </summary>
    /// <param name="id">Идентификатор владельца реестра</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Deleting registry holder: {RegistryHolderId}", id);
        
        if (!await registryHolderRepository.CanBeDeletedAsync(id, cancellationToken))
        {
            return Result.Fail(registryHolderErrorsFactory.CannotDeleteUsedRegistryHolder(id));
        }
        
        await registryHolderRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows == 0)
        {
            logger.Warning("No registry holder was deleted for id: {RegistryHolderId}", id);
        }
        return Result.Ok();
    }
}