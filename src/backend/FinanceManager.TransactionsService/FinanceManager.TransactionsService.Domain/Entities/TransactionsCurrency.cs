using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Domain.Entities;

public class TransactionsCurrency(
    string name,
    string charCode,
    string numCode,
    string? sign = null,
    string? emoji = null) : IdentityModel
{
    public string Name { get; set; } = name;

    public string CharCode { get; set; } = charCode;

    public string NumCode { get; set; } = numCode;

    public string? Sign { get; set; } = sign;

    public string? Emoji { get; set; } = emoji;
}