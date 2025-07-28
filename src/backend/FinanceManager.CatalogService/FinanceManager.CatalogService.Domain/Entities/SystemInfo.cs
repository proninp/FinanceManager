namespace FinanceManager.CatalogService.Domain.Entities;

/// <summary>
/// Value Object, содержащий информацию о системе и окружении выполнения приложения.
/// </summary>
/// <remarks>
/// Неизменяемый тип, представляющий снимок системных характеристик в определенный момент времени.
/// </remarks>
public sealed record SystemInfo()
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