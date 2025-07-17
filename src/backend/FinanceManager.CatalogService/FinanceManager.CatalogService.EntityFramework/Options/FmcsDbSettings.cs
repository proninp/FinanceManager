﻿namespace FinanceManager.CatalogService.EntityFramework.Options;

/// <summary>
/// Настройки подключения к базе данных.
/// </summary>
public class FmcsDbSettings
{
    /// <summary>
    /// Хост базы данных.
    /// </summary>
    public required string Host { get; set; }
    
    /// <summary>
    /// Порт для подключения к базе данных.
    /// </summary>
    public required string Port { get; set; }
    
    /// <summary>
    /// Имя базы данных.
    /// </summary>
    public required string Database { get; set; }
    
    /// <summary>
    /// Имя пользователя для подключения к базе данных.
    /// </summary>
    public required string Username { get; set; }
    
    /// <summary>
    /// Пароль для подключения к базе данных.
    /// </summary>
    public required string Password { get; set; }
    
    /// <summary>
    /// Формирует строку подключения к базе данных на основе текущих настроек.
    /// </summary>
    /// <returns>Строка подключения к базе данных.</returns>
    public string GetConnectionString()
    {
        return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";
    }
}
