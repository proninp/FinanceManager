using FinanceManager.CatalogService.Abstractions.Repositories;
using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.Categories;
using FinanceManager.CatalogService.Implementations.Errors.Abstractions;
using FluentResults;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Services;

/// <summary>
/// Сервис для управления категориями доходов и расходов
/// </summary>
public class CategoryService(
    IUnitOfWork unitOfWork,
    ICategoryRepository categoryRepository,
    ICategoryErrorsFactory errorsFactory,
    ILogger logger) : ICategoryService
{
    /// <summary>
    /// Получает категорию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с DTO категории или ошибкой</returns>
    public async Task<Result<CategoryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Получение категории по идентификатору: {CategoryId}", id);

        var category =
            await categoryRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (category is null)
        {
            logger.Warning("Категория с идентификатором {CategoryId} не найдена", id);
            return Result.Fail(errorsFactory.NotFound(id));
        }

        logger.Information("Категория {CategoryId} успешно получена", id);
        return Result.Ok(category.ToDto());
    }

    /// <summary>
    /// Получает список категорий с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком категорий или ошибкой</returns>
    public async Task<Result<ICollection<CategoryDto>>> GetPagedAsync(CategoryFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Получение списка категорий с фильтрацией: {@Filter}", filter);

        var categories = await categoryRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);
        var categoriesDto = categories.ToDto();

        logger.Information("Получено {Count} категорий", categoriesDto.Count);
        return Result.Ok(categoriesDto);
    }

    /// <summary>
    /// Получает все категории пользователя
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="includeRelated">Включать ли связанные сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком категорий пользователя или ошибкой</returns>
    public async Task<Result<ICollection<CategoryDto>>> GetByRegistryHolderIdAsync(Guid registryHolderId,
        bool includeRelated = true,
        CancellationToken cancellationToken = default)
    {
        logger.Information(
            "Получение категорий для владельца справочника: {RegistryHolderId}. Включить связанные данные: {IncludeRelated}",
            registryHolderId, includeRelated);

        var categories =
            await categoryRepository.GetByRegistryHolderIdAsync(registryHolderId, includeRelated, cancellationToken);
        var categoriesDto = categories.ToDto();

        logger.Information("Для владельца {RegistryHolderId} получено {Count} категорий",
            registryHolderId, categoriesDto.Count);
        return Result.Ok(categoriesDto);
    }

    /// <summary>
    /// Создает новую категорию
    /// </summary>
    /// <param name="createDto">Данные для создания категории</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданной категорией или ошибкой</returns>
    public async Task<Result<CategoryDto>> CreateAsync(CreateCategoryDto createDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Создание новой категории: {@CreateDto}", createDto);

        if (string.IsNullOrWhiteSpace(createDto.Name))
        {
            logger.Warning("Попытка создания категории без указания названия");
            return Result.Fail(errorsFactory.NameIsRequired());
        }

        // Проверка уникальности имени в рамках владельца и уровня иерархии
        var isUnique = await categoryRepository.IsNameUniqueInScopeAsync(
            createDto.RegistryHolderId, createDto.Name, createDto.ParentId, null, cancellationToken);
        if (!isUnique)
        {
            logger.Warning(
                "Попытка создания категории с неуникальным названием '{CategoryName}' для владельца {RegistryHolderId} и родителя {ParentId}",
                createDto.Name, createDto.RegistryHolderId, createDto.ParentId);
            return Result.Fail(errorsFactory.NameAlreadyExistsInScope(createDto.Name));
        }

        var category = await categoryRepository.AddAsync(createDto.ToCategory(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.Information(
            "Категория {CategoryId} с названием '{CategoryName}' успешно создана для владельца {RegistryHolderId}",
            category.Id, createDto.Name, createDto.RegistryHolderId);
        return Result.Ok(category.ToDto());
    }

    /// <summary>
    /// Обновляет существующую категорию
    /// </summary>
    /// <param name="updateDto">Данные для обновления категории</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленной категорией или ошибкой</returns>
    public async Task<Result<CategoryDto>> UpdateAsync(UpdateCategoryDto updateDto,
        CancellationToken cancellationToken = default)
    {
        logger.Information("Обновление категории: {@UpdateDto}", updateDto);

        var category = await categoryRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (category is null)
        {
            logger.Warning("Категория с идентификатором {CategoryId} не найдена для обновления", updateDto.Id);
            return Result.Fail(errorsFactory.NotFound(updateDto.Id));
        }

        var isNeedUpdate = false;

        if (!string.IsNullOrWhiteSpace(updateDto.Name) && !string.Equals(category.Name, updateDto.Name))
        {
            var isUnique = await categoryRepository.IsNameUniqueInScopeAsync(
                category.RegistryHolderId, updateDto.Name, category.ParentId, updateDto.Id, cancellationToken);
            if (!isUnique)
            {
                logger.Warning(
                    "Попытка обновления категории {CategoryId} с неуникальным названием '{CategoryName}' в рамках области действия",
                    updateDto.Id, updateDto.Name);
                return Result.Fail(errorsFactory.NameAlreadyExistsInScope(updateDto.Name));
            }

            category.Name = updateDto.Name;
            isNeedUpdate = true;
        }

        if (updateDto.Income.HasValue && category.Income != updateDto.Income.Value)
        {
            category.Income = updateDto.Income.Value;
            isNeedUpdate = true;
        }

        if (updateDto.Expense.HasValue && category.Expense != updateDto.Expense.Value)
        {
            category.Expense = updateDto.Expense.Value;
            isNeedUpdate = true;
        }

        if (updateDto.Emoji != null && !string.Equals(category.Emoji, updateDto.Emoji))
        {
            category.Emoji = updateDto.Emoji;
            isNeedUpdate = true;
        }

        if (updateDto.Icon != null && !string.Equals(category.Icon, updateDto.Icon))
        {
            category.Icon = updateDto.Icon;
            isNeedUpdate = true;
        }

        if (updateDto.ParentId != category.ParentId)
        {
            if (updateDto.ParentId.HasValue && updateDto.ParentId.Value != category.ParentId)
            {
                var isParentValid =
                    await categoryRepository.IsParentChangeValidAsync(category.Id, updateDto.ParentId,
                        cancellationToken);
                if (!isParentValid)
                {
                    logger.Warning(
                        "Попытка установки родительской категории {ParentId} для категории {CategoryId} приведет к циклической зависимости",
                        updateDto.ParentId.Value, category.Id);
                    return Result.Fail(
                        errorsFactory.RecursiveParentCategoryRelation(category.Id, updateDto.ParentId.Value));
                }
            }

            category.ParentId = updateDto.ParentId;
            isNeedUpdate = true;
        }

        if (isNeedUpdate)
        {
            // нам не нужно вызывать метод categoryRepository.UpdateAsync(), так как сущность category уже отслеживается
            await unitOfWork.CommitAsync(cancellationToken);
            logger.Information("Категория {CategoryId} успешно обновлена", category.Id);
        }
        else
        {
            logger.Information("Изменения для категории {CategoryId} не обнаружены", updateDto.Id);
        }

        return Result.Ok(category.ToDto());
    }

    /// <summary>
    /// Удаляет категорию
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.Information("Удаление категории: {CategoryId}", id);
        
        await categoryRepository.DeleteAsync(id, cancellationToken);
        var affectedRows = await unitOfWork.CommitAsync(cancellationToken);

        if (affectedRows > 0)
        {
            logger.Information("Категория {CategoryId} успешно удалена", id);
        }
        
        return Result.Ok();
    }
}