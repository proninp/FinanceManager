namespace FinanceManager.CatalogService.Contracts.DTOs.Categories;

/// <summary>
/// DTO для фильтрации и пагинации категорий
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="RegistryHolderId">Идентификатор владельца категории</param>
/// <param name="Name">Название категории</param>
/// <param name="Income">Фильтр по доходным категориям</param>
/// <param name="Expense">Фильтр по расходным категориям</param>
/// <param name="Emoji">Эмодзи категории</param>
/// <param name="HasIcon">Указана ли иконка категории</param>
/// <param name="HasIcon">Существуют ли зарегистрированные транзакции по данной категории</param>
/// <param name="ParentId">Идентификатор родительской категории</param>
public record CategoryFilterDto(
    int ItemsPerPage,
    int Page,
    Guid? RegistryHolderId,
    string? Name,
    bool? Income,
    bool? Expense,
    string? Emoji,
    bool? HasIcon,
    bool? HasTransactions,
    Guid? ParentId
);