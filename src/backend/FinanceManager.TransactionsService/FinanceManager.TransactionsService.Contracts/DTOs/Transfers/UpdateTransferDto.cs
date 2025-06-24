namespace FinanceManager.TransactionsService.Contracts.DTOs.Transfers;

/// <summary>
/// DTO для обновления перевода между счетами
/// </summary>
/// <param name="Id">Уникальный идентификатор перевода</param>
/// <param name="Date">Дата осуществления перевода</param>
/// <param name="FromAccountId">Идентификатор счёта списания</param>
/// <param name="ToAccountId">Идентификатор счёта зачисления</param>
/// <param name="FromAmount">Сумма списания со счёта отправителя</param>
/// <param name="ToAmount">Сумма зачисления на счёт получателя</param>
/// <param name="Description">Описание перевода (необязательно)</param>
public record UpdateTransferDto(
    Guid Id,
    DateTime? Date = null,
    Guid? FromAccountId = null,
    Guid? ToAccountId = null,
    decimal? FromAmount = null,
    decimal? ToAmount = null,
    string? Description = null
);