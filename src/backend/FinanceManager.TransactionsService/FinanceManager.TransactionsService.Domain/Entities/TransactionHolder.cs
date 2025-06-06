using FinanceManager.TransactionsService.Domain.Abstractions;
using FinanceManager.TransactionsService.Domain.Enums;

namespace FinanceManager.TransactionsService.Domain.Entities;

public class TransactionHolder(Role role, long? telegramId = null):IdentityModel
{
    public Role Role { get; set; }
    
    public long? TelegramId { get; set; }
}