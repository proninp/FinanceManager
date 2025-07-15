namespace FinanceManager.CatalogService.Abstractions.Repositories.Common;

public interface IDeletableValidator
{
    /// <summary>
    /// Проверяет, может ли сущность с указанным Id быть удалена
    /// </summary>
    /// <param name="id">Идентификатор сущеости</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если сущность может быть удалена</returns>
    Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default);
}