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
        logger.Information("Getting category by id: {CategoryId}", id);
        var category =
            await categoryRepository.GetByIdAsync(id, disableTracking: true, cancellationToken: cancellationToken);
        if (category is null)
        {
            return Result.Fail(errorsFactory.NotFound(id));
        }

        logger.Information("Successfully retrieved category: {CategoryId}", id);
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
        logger.Information("Getting paged categories with filter: {@Filter}", filter);
        var categories = await categoryRepository.GetPagedAsync(filter, cancellationToken: cancellationToken);
        var categoriesDto = categories.ToDto();
        logger.Information("Successfully retrieved {Count} categories", categoriesDto.Count);
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
        logger.Information("Getting categories for registry holder: {RegistryHolderId}", registryHolderId);
        var categories =
            await categoryRepository.GetByRegistryHolderIdAsync(registryHolderId, includeRelated, cancellationToken);
        var categoriesDto = categories.ToDto();
        logger.Information("Successfully retrieved {Count} categories for registry holder: {RegistryHolderId}",
            categoriesDto.Count, registryHolderId);
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
        logger.Information("Creating category: {@CreateDto}", createDto);

        if (string.IsNullOrWhiteSpace(createDto.Name))
        {
            return Result.Fail(errorsFactory.NameIsRequired());
        }

        // Проверка уникальности имени в рамках владельца и уровня иерархии
        var isUnique = await categoryRepository.IsNameUniqueInScopeAsync(
            createDto.RegistryHolderId, createDto.Name, createDto.ParentId, null, cancellationToken);
        if (!isUnique)
        {
            return Result.Fail(errorsFactory.NameAlreadyExistsInScope(createDto.Name));
        }

        var category = await categoryRepository.AddAsync(createDto.ToCategory(), cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        logger.Information("Successfully created category: {CategoryId}", category.Id);
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
        logger.Information("Updating category: {@UpdateDto}", updateDto);

        var category = await categoryRepository.GetByIdAsync(updateDto.Id, cancellationToken: cancellationToken);
        if (category is null)
        {
            return Result.Fail(errorsFactory.NotFound(updateDto.Id));
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Name) && !string.Equals(category.Name, updateDto.Name))
        {
            var isUnique = await categoryRepository.IsNameUniqueInScopeAsync(
                category.RegistryHolderId, updateDto.Name, category.ParentId, updateDto.Id, cancellationToken);
            if (!isUnique)
            {
                return Result.Fail(errorsFactory.NameAlreadyExistsInScope(updateDto.Name));
            }

            category.Name = updateDto.Name;
        }

        if (updateDto.Income.HasValue)
            category.Income = updateDto.Income.Value;
        if (updateDto.Expense.HasValue)
            category.Expense = updateDto.Expense.Value;
        if (updateDto.Emoji != null)
            category.Emoji = updateDto.Emoji;
        if (updateDto.Icon != null)
            category.Icon = updateDto.Icon;

        if (updateDto.ParentId != category.ParentId)
        {
            if (updateDto.ParentId.HasValue && updateDto.ParentId.Value != category.ParentId)
            {
                var isParentValid =
                    await categoryRepository.IsParentChangeValidAsync(category.Id, updateDto.ParentId, 
                        cancellationToken);
                if (!isParentValid)
                {
                    return Result.Fail(
                        errorsFactory.RecursiveParentCategoryRelation(category.Id, updateDto.ParentId.Value));
                }
            }
            category.ParentId = updateDto.ParentId;
        }

        // нам не нужно вызывать метод categoryRepository.UpdateAsync(), так как сущность category уже отслеживается
        await unitOfWork.CommitAsync(cancellationToken);
        
        logger.Information("Successfully updated category: {CategoryId}", category.Id);
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
        logger.Information("Deleting category: {CategoryId}", id);

        var category = await categoryRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (category is null)
        {
            return Result.Ok();
        }

        await categoryRepository.DeleteAsync(id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        logger.Information("Successfully deleted category: {CategoryId}", id);
        return Result.Ok();
    }
}