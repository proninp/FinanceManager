namespace FinanceManager.CatalogService.Contracts.DTOs.SystemInfo;

/// <summary>
/// DTO для ответа с информацией о системе
/// </summary>
public sealed record SystemInfoResponseDto
{
    /// <summary>
    /// Имя текущей исполняемой сборки.
    /// </summary>
    public required string AssemblyName { get; init; }
    
    /// <summary>
    /// Версия сборки в формате Major.Minor.Build.Revision.
    /// </summary>
    public required string AssemblyVersion { get; init; }
    
    /// <summary>
    /// Описание версии .NET (например, ".NET 8.0.1").
    /// </summary>
    public required string FrameworkDescription { get; init; }
    
    /// <summary>
    /// Метка времени сбора информации в UTC.
    /// </summary>
    public required DateTime Timestamp { get; init; }
    
    /// <summary>
    /// Имя компьютера (hostname).
    /// </summary>
    public required string MachineName { get; init; }
    
    /// <summary>
    /// Архитектура ОС (x64, Arm64 и т.д.).
    /// </summary>
    public required string OsArchitecture { get; init; }
    
    /// <summary>
    /// Тип ОС (например, "Linux 5.15.0").
    /// </summary>
    public required string OsPlatform { get; init; }
    
    /// <summary>
    /// Версия ОС в формате, специфичном для платформы.
    /// </summary>
    public required string OsVersion { get; init; }
    
    /// <summary>
    /// Архитектура процесса (x64, Wasm и т.д.).
    /// </summary>
    public required string ProcessArchitecture { get; init; }
    
    
}

/// <summary>
/// Расширения для преобразования объектов информации о системе в DTO.
/// </summary>
public static class SystemInfoResponseExtensions
{
    /// <summary>
    /// Преобразует доменный объект <see cref="Domain.Entities.SystemInfo"/> в DTO <see cref="SystemInfoResponseDto"/>.
    /// </summary>
    /// <param name="systemInfo">Доменный объект с информацией о системе.</param>
    /// <returns>DTO с информацией о системе.</returns>
    /// <remarks>
    /// Выполняет простое копирование всех свойств. Для сложных преобразований рекомендуется использовать AutoMapper.
    /// </remarks>
    public static SystemInfoResponseDto ToDto(this Domain.Entities.SystemInfo systemInfo) => new()
    {
        AssemblyName = systemInfo.AssemblyName,
        AssemblyVersion = systemInfo.AssemblyVersion,
        FrameworkDescription = systemInfo.FrameworkDescription,
        Timestamp = systemInfo.Timestamp,
        MachineName = systemInfo.MachineName,
        OsArchitecture = systemInfo.OsArchitecture,
        OsPlatform = systemInfo.OsPlatform,
        OsVersion = systemInfo.OsVersion,
        ProcessArchitecture = systemInfo.ProcessArchitecture
    };
}