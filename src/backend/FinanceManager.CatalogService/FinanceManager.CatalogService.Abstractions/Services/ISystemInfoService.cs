using FinanceManager.CatalogService.Contracts.DTOs.SystemInfo;
using FinanceManager.CatalogService.Domain.Entities;

namespace FinanceManager.CatalogService.Abstractions.Services;

/// <summary>
/// Интерфейс сервиса для получения системной информации
/// </summary>
public interface ISystemInfoService
{
    /// <summary>
    /// Получает информацию о системе и приложении
    /// </summary>
    /// <returns>Информация о системе</returns>
    SystemInfoResponseDto GetSystemInfo();
}