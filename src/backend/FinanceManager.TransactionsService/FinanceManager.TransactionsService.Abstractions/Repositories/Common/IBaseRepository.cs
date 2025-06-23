using FinanceManager.TransactionsService.Domain.Abstractions;

namespace FinanceManager.TransactionsService.Abstractions.Repositories.Common;

public interface IBaseRepository<T, in TFilterDto> where T:IdentityModel
{
    
}