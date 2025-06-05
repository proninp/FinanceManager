using System.ComponentModel.DataAnnotations;
using FinanceManager.CatalogService.Contracts.Common;

namespace FinanceManager.CatalogService.Contracts.DTOs.Abstractions;

/// <summary>
/// Базовый абстрактный record для пагинации
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице (от 1 до 100)</param>
/// <param name="Page">Номер страницы (начиная с 1)</param>
public abstract record BasePaginationDto(
    [Range(PaginationDefaults.MinItemsPerPage, PaginationDefaults.MaxItemsPerPage, 
        ErrorMessage = "Количество элементов на странице должно быть от 1 до 100")]
    int ItemsPerPage = PaginationDefaults.DefaultItemsPerPage,
    
    [Range(PaginationDefaults.DefaultPage, int.MaxValue, 
        ErrorMessage = "Номер страницы должен быть больше 0")]
    int Page = PaginationDefaults.DefaultPage
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