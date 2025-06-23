using FinanceManager.TransactionsService.Contracts.DTOs.Abstractions;
using FinanceManager.TransactionsService.Domain.Enums;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;

/// <summary>
/// DTO для фильтрации и пагинации владельцев транзакций
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="TelegramId">Идентификатор пользователя в Telegram</param>
/// <param name="Role">Роль пользователя в системе</param>
public record TransactionHolderFilterDto(
    int ItemsPerPage,
    int Page,
    long? TelegramId = null,
    Role? Role = null
) : BasePaginationDto(ItemsPerPage, Page);