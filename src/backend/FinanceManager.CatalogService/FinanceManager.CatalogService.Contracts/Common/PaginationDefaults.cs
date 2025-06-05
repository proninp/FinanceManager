namespace FinanceManager.CatalogService.Contracts.Common;

/// <summary>
/// Константы для пагинации
/// </summary>
public static class PaginationDefaults
{
    /// <summary>
    /// Количество элементов на странице по умолчанию
    /// </summary>
    public const int DefaultItemsPerPage = 20;
    
    /// <summary>
    /// Номер страницы по умолчанию
    /// </summary>
    public const int DefaultPage = 1;
    
    /// <summary>
    /// Минимальное количество элементов на странице
    /// </summary>
    public const int MinItemsPerPage = 1;
    
    /// <summary>
    /// Максимальное количество элементов на странице
    /// </summary>
    public const int MaxItemsPerPage = 100;
}