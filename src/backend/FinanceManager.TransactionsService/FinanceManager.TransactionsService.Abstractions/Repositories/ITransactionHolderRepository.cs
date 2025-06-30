using FinanceManager.TransactionsService.Abstractions.Repositories.Common;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionHolders;
using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Abstractions.Repositories;

public interface ITransactionHolderRepository : IBaseRepository<TransactionHolder, TransactionHolderFilterDto>
{
    
}