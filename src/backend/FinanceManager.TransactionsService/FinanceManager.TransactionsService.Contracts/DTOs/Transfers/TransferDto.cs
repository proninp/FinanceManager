using FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;

namespace FinanceManager.TransactionsService.Contracts.DTOs.Transfers;

/// <summary>
/// DTO для представления перевода между счетами
/// </summary>
/// <param name="Id">Уникальный идентификатор перевода</param>
/// <param name="Date">Дата осуществления перевода</param>
/// <param name="FromAccount">Счет, с которого осуществляется перевод</param>
/// <param name="ToAccount">Счет, на который зачисляются средства</param>
/// <param name="FromAmount">Сумма списания со счёта отправителя</param>
/// <param name="ToAmount">Сумма зачисления на счёт получателя</param>
/// <param name="Description">Описание перевода (необязательно)</param>
public record TransferDto(
    Guid Id,
    DateTime Date,
    TransactionAccountDto FromAccount,
    TransactionAccountDto ToAccount,
    decimal FromAmount,
    decimal ToAmount,
    string? Description
    );