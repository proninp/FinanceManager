using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionCurrencies;

/// <summary>
/// DTO для создания валюты
/// </summary>
/// <param name="Name">Название валюты</param>
/// <param name="CharCode">Символьный код валюты</param>
/// <param name="NumCode">Числовой код валюты</param>
public record CreateTransactionCurrencyDto(
    string Name,
    string CharCode,
    string NumCode
);

public static class CreateTransactionCurrencyDtoExtensions
{
    /// <summary>
    /// Преобразует CreateTransactionCurrencyDto в TransactionsCurrency
    /// </summary>
    public static TransactionsCurrency ToCurrency(this CreateTransactionCurrencyDto dto)
    {
        return new TransactionsCurrency(
            charCode: dto.CharCode,
            numCode: dto.NumCode
        );

    }
}