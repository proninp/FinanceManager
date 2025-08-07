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
        logger.Information("Получение владельца реестра по идентификатору: {RegistryHolderId}", id);

        var holder =
            await registryHolderRepository.GetByIdAsync(id, disableTracking: true,
                cancellationToken: cancellationToken);
        if (holder is null)
        {
            logger.Warning("Владелец реестра с идентификатором {RegistryHolderId} не найден", id);
            return Result.Fail(registryHolderErrorsFactory.NotFound(id));
        }

        logger.Information("Владелец реестра {RegistryHolderId} успешно получен", id);
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
        logger.Information("Получение списка владельцев реестра с фильтрацией: {@Filter}", filter);
        var holders = await registryHolderRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);

        var holdersDto = holders.ToDto();

        logger.Information("Получено {Count} владельцев реестра", holdersDto.Count);
        return Result.Ok(holdersDto);
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
        logger.Information("Создание нового владельца реестра: {@CreateDto}", createDto);

        if (createDto.TelegramId == 0)
        {
            logger.Warning("Попытка создания владельца реестра без указания Telegram ID");
            return Result.Fail(registryHolderErrorsFactory.TelegramIdIsRequired());
        }

        if (!await registryHolderRepository.IsTelegramIdUniqueAsync(createDto.TelegramId, cancellationToken: cancellationToken))
        {
            logger.Warning("Попытка создания владельца реестра с неуникальным Telegram ID: {TelegramId}", createDto.TelegramId);
            return Result.Fail(registryHolderErrorsFactory.TelegramIdAlreadyExists(createDto.TelegramId));
        }

        var holder = await registryHolderRepository.AddAsync(createDto.ToRegistryHolder(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information("Владелец реестра {RegistryHolderId} с Telegram ID {TelegramId} успешно создан",
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
        logger.Information("Обновление владельца реестра: {@UpdateDto}", updateDto);

        var holder = await registryHolderRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (holder is null)
        {
            logger.Warning("Владелец реестра с идентификатором {RegistryHolderId} не найден для обновления", updateDto.Id);
            return Result.Fail(registryHolderErrorsFactory.NotFound(updateDto.Id));
        }

        var isNeedUpdate = false;

        if (updateDto.TelegramId.HasValue && holder.TelegramId != updateDto.TelegramId.Value)
        {
            if (updateDto.TelegramId.Value <= 0)
            {
                logger.Warning("Попытка обновления владельца реестра {RegistryHolderId} с некорректным Telegram ID", updateDto.Id);
                return Result.Fail(registryHolderErrorsFactory.TelegramIdIsRequired());
            }

            if (!await registryHolderRepository.IsTelegramIdUniqueAsync(updateDto.TelegramId.Value, updateDto.Id, cancellationToken))
            {
                logger.Warning("Попытка обновления владельца реестра {RegistryHolderId} с неуникальным Telegram ID: {TelegramId}", updateDto.Id, updateDto.TelegramId.Value);
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
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Владелец реестра {RegistryHolderId} успешно обновлен", updateDto.Id);
        }
        else
        {
            logger.Information("Изменения для владельца реестра {RegistryHolderId} не обнаружены", updateDto.Id);
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
        logger.Information("Удаление владельца реестра: {RegistryHolderId}", id);
        
        if (!await registryHolderRepository.CanBeDeletedAsync(id, cancellationToken))
        {
            logger.Warning("Владелец реестра {RegistryHolderId} не может быть удален, так как используется в других сущностях", id);
            return Result.Fail(registryHolderErrorsFactory.CannotDeleteUsedRegistryHolder(id));
        }
        
        await registryHolderRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);
        if (affectedRows > 0)
        {
            logger.Information("Владелец реестра {RegistryHolderId} успешно удален", id);
        }
        
        return Result.Ok();
    }
}