using FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;
using FinanceManager.TransactionsService.Domain.Entities;

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

/// <summary>
/// Методы-расширения для преобразования сущности Transfer в TransferDto
/// </summary>
public static class TransferDtoExtensions
{
    /// <summary>
    /// Преобразует сущность Transfer в DTO TransferDto
    /// </summary>
    /// <param name="transfer">Сущность перевода денежных средств между двумя счетами</param>
    /// <returns>Экземпляр TransferDto</returns>
    public static TransferDto ToDto(this Transfer transfer)
    {
        return new TransferDto(
            transfer.Id,
            transfer.Date,
            transfer.FromAccount.ToDto(),
            transfer.ToAccount.ToDto(),
            transfer.FromAmount,
            transfer.ToAmount,
            transfer.Description
        );
    }

    /// <summary>
    /// Преобразует коллекцию сущностей Transfer в коллекцию DTO TransferDto
    /// </summary>
    /// <param name="transfers">Коллекция сущностей переводов денежных средств</param>
    /// <returns>Коллекция TransferDto</returns>
    public static ICollection<TransferDto> ToDto(this IEnumerable<Transfer> transfers) =>
        transfers.Select(ToDto).ToList();
}