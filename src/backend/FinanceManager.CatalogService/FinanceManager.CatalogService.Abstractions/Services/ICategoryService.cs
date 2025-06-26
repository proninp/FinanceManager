﻿using FinanceManager.CatalogService.Contracts.DTOs.Categories;
using FluentResults;

namespace FinanceManager.CatalogService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для работы с категориями доходов и расходов
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Получает категорию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с данными категории или ошибкой</returns>
    Task<Result<CategoryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список категорий с фильтрацией и пагинацией
    /// </summary>
    /// <param name="filter">Параметры фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком категорий или ошибкой</returns>
    Task<Result<ICollection<CategoryDto>>> GetPagedAsync(
        CategoryFilterDto filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все категории пользователя
    /// </summary>
    /// <param name="registryHolderId">Идентификатор владельца</param>
    /// <param name="includeRelated">Включать ли связанные сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком категорий пользователя или ошибкой</returns>
    Task<Result<ICollection<CategoryDto>>> GetByRegistryHolderIdAsync(
        Guid registryHolderId,
        bool includeRelated = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создает новую категорию
    /// </summary>
    /// <param name="createDto">Данные для создания категории</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданной категорией или ошибкой</returns>
    Task<Result<CategoryDto>> CreateAsync(
        CreateCategoryDto createDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую категорию
    /// </summary>
    /// <param name="updateDto">Данные для обновления категории</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленной категорией или ошибкой</returns>
    Task<Result<CategoryDto>> UpdateAsync(
        UpdateCategoryDto updateDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет категорию
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции</returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}