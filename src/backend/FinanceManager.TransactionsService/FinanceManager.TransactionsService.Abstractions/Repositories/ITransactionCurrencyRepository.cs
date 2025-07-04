using FinanceManager.TransactionsService.Abstractions.Repositories.Common;
using FinanceManager.TransactionsService.Contracts.DTOs.TransactionCurrencies;
using FinanceManager.TransactionsService.Domain.Entities;

namespace FinanceManager.TransactionsService.Abstractions.Repositories;

public interface ITransactionCurrencyRepository: IBaseRepository<TransactionsCurrency, TransactionCurrencyFilterDto>
{
    
}