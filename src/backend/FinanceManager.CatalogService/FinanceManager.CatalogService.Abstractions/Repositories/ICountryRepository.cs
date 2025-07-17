using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Contracts.DTOs.Countries;
using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Abstractions.Repositories;

public interface ICountryRepository : IBaseRepository<Country, CountryFilterDto>, IInitializerRepository<Country>,
    IDeletableValidator
{
    /// <summary>
    /// Получает все страны, отсортированные по названию
    /// </summary>
    /// <param name="ascending">Направление сортировки (по возрастанию по умолчанию)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список всех стран, отсортированный по названию</returns>
    Task<ICollection<Country>> GetAllOrderedByNameAsync(bool ascending = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет уникальность названия страны
    /// </summary>
    /// <param name="name">Название страны</param>
    /// <param name="excludeId">Исключить страну с данным ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если название уникально</returns>
    Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
}