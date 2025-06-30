using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;

/// <summary>
/// DTO для создания категории
/// </summary>
/// <param name="TransactionHolderId">Идентификатор владельца категории</param>
/// <param name="Income">Является ли категория доходной</param>
/// <param name="Expense">Является ли категория расходной</param>
public record CreateTransactionCategoryDto(
    Guid TransactionHolderId,
    bool Income,
    bool Expense
);

public static class CreateTransactionCategoryDtoExtensions
{
    /// <summary>
    /// Преобразует CreateTransactionCategoryDto в TransactionsCategory
    /// </summary>
    public static TransactionsCategory ToCategory(this CreateTransactionCategoryDto dto)
    {
        return new TransactionsCategory(
            holderId: dto.TransactionHolderId,
            income: dto.Income,
            expense: dto.Expense
        );
    }
}