namespace FinanceManager.TransactionsService.Contracts.DTOs.Transfers;

/// <summary>
/// DTO для создания перевода между счетами
/// </summary>
/// <param name="Date">Дата осуществления перевода</param>
/// <param name="FromAccountId">Идентификатор счёта списания</param>
/// <param name="ToAccountId">Идентификатор счёта зачисления</param>
/// <param name="FromAmount">Сумма списания со счёта отправителя</param>
/// <param name="ToAmount">Сумма зачисления на счёт получателя</param>
/// <param name="Description">Описание перевода (необязательно)</param>
public record CreateTransferDto(
    DateTime Date,
    Guid FromAccountId,
    Guid ToAccountId,
    decimal FromAmount,
    decimal ToAmount,
    string? Description
);