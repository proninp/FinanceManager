using System.ComponentModel.DataAnnotations;
using FinanceManager.CatalogService.Contracts.Common;

namespace FinanceManager.CatalogService.Contracts.DTOs.Abstractions;

/// <summary>
/// Базовый абстрактный record для пагинации
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице (от 1 до 100)</param>
/// <param name="Page">Номер страницы (начиная с 1)</param>
public abstract record BasePaginationDto(
    [Range(PaginationDefaults.MinItemsPerPage, int.MaxValue, 
        ErrorMessage = "Количество элементов на странице должно быть больше 0")]
    int ItemsPerPage,
    
    [Range(PaginationDefaults.DefaultPage, int.MaxValue, 
        ErrorMessage = "Номер страницы должен быть больше 0")]
    int Page
)
{
    /// <summary>
    /// Вычисляет количество элементов для пропуска (для OFFSET в SQL)
    /// </summary>
    public int Skip => (Page - 1) * ItemsPerPage;
    
    /// <summary>
    /// Возвращает количество элементов для выборки (для LIMIT в SQL)
    /// </summary>
    public int Take => ItemsPerPage;
}