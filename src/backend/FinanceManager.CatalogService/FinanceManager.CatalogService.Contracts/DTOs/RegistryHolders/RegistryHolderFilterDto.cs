using FinanceManager.CatalogService.Contracts.DTOs.Abstractions;
using FinanceManager.CatalogService.Domain.Enums;

namespace FinanceManager.CatalogService.Contracts.DTOs.RegistryHolders;

/// <summary>
/// DTO для фильтрации и пагинации владельцев реестра
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="TelegramId">Идентификатор пользователя в Telegram</param>
/// <param name="Role">Роль пользователя в системе</param>
public record RegistryHolderFilterDto(
    int ItemsPerPage,
    int Page,
    long? TelegramId = null,
    Role? Role = null
) : BasePaginationDto(ItemsPerPage, Page);