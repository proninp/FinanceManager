using System.Reflection;
using System.Runtime.InteropServices;
using FinanceManager.CatalogService.Abstractions.Services;
using FinanceManager.CatalogService.Contracts.DTOs.SystemInfo;
using FinanceManager.CatalogService.Domain.Entities;
using Serilog;

namespace FinanceManager.CatalogService.Implementations.Services;

/// <summary>
/// Сервис для получения информации о системе и окружении выполнения приложения.
/// </summary>
/// <remarks>
/// Реализует <see cref="ISystemInfoService"/>, предоставляя актуальные данные:
/// - О сборке приложения
/// - О платформе .NET
/// - О характеристиках операционной системы
/// - О параметрах процесса выполнения
/// </remarks>
public sealed class SystemInfoService(ILogger logger) : ISystemInfoService
{
    /// <summary>
    /// Получает текущую системную информацию.
    /// </summary>
    /// <returns>Value Object <see cref="SystemInfoResponseDto"/> с данными о системе.</returns>
    /// <exception cref="Exception">Может выбрасывать исключения при критических ошибках сбора информации.</exception>
    public SystemInfoResponseDto GetSystemInfo()
    {
        try
        {
            logger.Information("Сбор системной информации");

            var assembly = Assembly.GetEntryAssembly();
            var systemInfo = new SystemInfo
            {
                AssemblyName = assembly?.GetName().Name ?? "Unknown",
                AssemblyVersion = assembly?.GetName().Version?.ToString() ?? "Unknown",
                FrameworkDescription = RuntimeInformation.FrameworkDescription,
                Timestamp = DateTime.UtcNow,
                MachineName = Environment.MachineName,
                OsArchitecture = RuntimeInformation.OSArchitecture.ToString(),
                OsPlatform = RuntimeInformation.OSDescription,
                OsVersion = Environment.OSVersion.VersionString,
                ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString()
            };

            logger.Information("Системная информация успешно собрана для {AssemblyName}", systemInfo.AssemblyName);
            return systemInfo.ToDto();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Ошибка при сборе системной информации");
            throw;
        }
    }
}