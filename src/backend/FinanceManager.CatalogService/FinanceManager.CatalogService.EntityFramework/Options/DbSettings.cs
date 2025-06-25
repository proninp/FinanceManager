﻿namespace FinanceManager.CatalogService.EntityFramework.Options;

/// <summary>
/// Настройки подключения к базе данных.
/// </summary>
public class DbSettings
{
    /// <summary>
    /// Строка подключения к базе данных.
    /// </summary>
    public required string DbConnectionString { get; set; }
}
