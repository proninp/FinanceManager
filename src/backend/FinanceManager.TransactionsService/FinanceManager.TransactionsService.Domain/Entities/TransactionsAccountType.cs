using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

public class TransactionsAccountType(string code, string description):IdentityModel
{
    public string Code { get; set; } = code;
    
    public string Description { get; set; } = description;
}