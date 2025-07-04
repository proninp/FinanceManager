using FinanceManager.TransactionsService.Abstractions.Repositories.Common;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionAccounts;
using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Abstractions.Repositories;

public interface ITransactionAccountRepository : IBaseRepository<TransactionsAccount, TransactionAccountFilterDto>
{
    
}