namespace FinanceManager.CatalogService.Implementations.Extensions;

/// <summary>
/// Предоставляет методы-расширения для работы со строками
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Возвращает строку с первой заглавной буквой
    /// </summary>
    /// <param name="input">Исходная строка</param>
    /// <returns>Строка, в которой первый символ преобразован в верхний регистр</returns>
    /// <exception cref="ArgumentNullException">Если <paramref name="input"/> равен null</exception>
    /// <exception cref="ArgumentException">Если <paramref name="input"/> пустая строка</exception>
    public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };
}