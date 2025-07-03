using FinanceManager.TransactionsService.Domain.Entities;
using FinanceManager.TransactionsService.Domain.Enums;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;

/// <summary>
/// DTO для владельца транзакций
/// </summary>
/// <param name="Id">Идентификатор владельца транзакций</param>
/// <param name="TelegramId">Идентификатор пользователя в Telegram</param>
/// <param name="Role">Роль пользователя в системе</param>
public record TransactionHolderDto(
    Guid Id,
    Role Role,
    long? TelegramId
    );
/// <summary>
/// Методы-расширения для преобразования сущности TransactionHolder в TransactionHolderDto
/// </summary>
public static class TransactionHolderDtoExtensions
{
    /// <summary>
    /// Преобразует сущность TransactionHolder в DTO TransactionHolderDto
    /// </summary>
    /// <param name="holder">Сущность участника системы, владельца транзакции</param>
    /// <returns>Экземпляр TransactionHolderDto</returns>
    public static TransactionHolderDto ToDto(this TransactionHolder holder)
    {
        return new TransactionHolderDto(
            holder.Id,
            holder.Role,
            holder.TelegramId
        );
    }
    /// <summary>
    /// Преобразует коллекцию сущностей TransactionHolder в коллекцию DTO TransactionHolderDto
    /// </summary>
    /// <param name="holders">Коллекция сущностей участников системы</param>
    /// <returns>Коллекция TransactionHolderDto</returns>
    public static ICollection<TransactionHolderDto> ToDto(this IEnumerable<TransactionHolder> holders) =>
        holders.Select(ToDto).ToList();
}