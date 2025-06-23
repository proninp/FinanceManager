using FinanceManager.TransactionsService.Contracts.DTOs.Abstractions;

namespace FinanceManager.TransactionsService.Contracts.DTOs.Transfers;

/// <summary>
/// DTO для фильтрации и пагинации списка переводов между счетами
/// </summary>
/// <param name="ItemsPerPage">Количество элементов на странице</param>
/// <param name="Page">Номер страницы</param>
/// <param name="DateFrom">Дата начала перевода (включительно)</param>
/// <param name="DateTo">Дата окончания перевода (включительно)</param>
/// <param name="FromAccountId">Фильтр по идентификатору счёта списания</param>
/// <param name="ToAccountId">Фильтр по идентификатору счёта зачисления</param>
/// <param name="FromAmountFrom">Минимальная сумма перевода со счёта отправителя</param>
/// <param name="FromAmountTo">Максимальная сумма перевода со счёта отправителя</param>
/// <param name="ToAmountFrom">Минимальная сумма зачисления на счёт получателя</param>
/// <param name="ToAmountTo">Максимальная сумма зачисления на счёт получателя</param>
/// <param name="DescriptionContains">Фильтр по содержанию текста в описании перевода</param>
public record TransferFilterDto(
    int ItemsPerPage,
    int Page,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    Guid? FromAccountId = null,
    Guid? ToAccountId = null,
    decimal? FromAmountFrom = null,
    decimal? FromAmountTo = null,
    decimal? ToAmountFrom = null,
    decimal? ToAmountTo = null,
    string? DescriptionContains = null
) : BasePaginationDto(ItemsPerPage, Page);