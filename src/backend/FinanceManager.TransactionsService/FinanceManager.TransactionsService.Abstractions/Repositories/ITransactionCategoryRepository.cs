using FinanceManager.TransactionsService.Abstractions.Repositories.Common;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionsCategories;
using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Abstractions.Repositories;

public interface ITransactionCategoryRepository : IBaseRepository<TransactionsCategory, TransactionCategoryFilterDto>
{
    
}