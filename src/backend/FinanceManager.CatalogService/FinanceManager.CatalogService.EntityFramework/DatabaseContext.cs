using FinanceManager.CatalogService.Abstractions.Repositories.Common;
using FinanceManager.CatalogService.Domain.Abstractions;
using FinanceManager.CatalogService.Domain.Entities;
using FinanceManager.CatalogService.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinanceManager.CatalogService.EntityFramework;

/// <summary>
/// Контекст базы данных для работы с сущностями каталога и управления транзакциями.
/// </summary>
public class DatabaseContext : DbContext, IUnitOfWork
{
    private readonly DbSettings _options;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DatabaseContext"/>.
    /// </summary>
    /// <param name="options">Параметры подключения к базе данных.</param>
    public DatabaseContext(IOptionsSnapshot<DbSettings> options)
    {
        _options = options.Value;
    }

    #region DbSets

    /// <summary>
    /// Счета.
    /// </summary>
    public DbSet<Account> Accounts { get; set; }
    
    /// <summary>
    /// Типы счетов.
    /// </summary>
    public DbSet<AccountType> AccountTypes { get; set; }
    
    /// <summary>
    /// Банки.
    /// </summary>
    public DbSet<Bank> Banks { get; set; }
    
    /// <summary>
    /// Категории.
    /// </summary>
    public DbSet<Category> Categories { get; set; }
    
    /// <summary>
    /// Страны.
    /// </summary>
    public DbSet<Country> Countries { get; set; }
    
    /// <summary>
    /// Валюты.
    /// </summary>
    public DbSet<Currency> Currencies { get; set; }
    
    /// <summary>
    /// Курсы валют.
    /// </summary>
    public DbSet<ExchangeRate> ExchageRates { get; set; }
    
    /// <summary>
    /// Владельцы справочников.
    /// </summary>
    public DbSet<RegistryHolder> RegistryHolders { get; set; }

    #endregion

    /// <summary>
    /// Конфигурирует параметры подключения к базе данных.
    /// </summary>
    /// <param name="optionsBuilder">Построитель параметров подключения.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_options.DbConnectionString);
        }
    }

    /// <summary>
    /// Применяет конфигурации моделей при создании схемы базы данных.
    /// </summary>
    /// <param name="modelBuilder">Построитель моделей.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
    }

    /// <summary>
    /// Сохраняет все изменения в контексте базы данных, предварительно устанавливая временные метки для новых сущностей.
    /// </summary>
    /// <returns>Количество затронутых записей.</returns>
    public override int SaveChanges()
    {
        SetTimeStamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Асинхронно сохраняет все изменения в контексте базы данных, предварительно устанавливая временные метки для новых сущностей.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Задача, возвращающая количество затронутых записей.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetTimeStamps();
        return base.SaveChangesAsync(cancellationToken);
    }
    
    /// <summary>
    /// Сохраняет все изменения в базе данных в рамках текущей транзакции.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Количество затронутых записей.</returns>
    public async Task<int> CommitAsync(CancellationToken cancellationToken) =>
        await SaveChangesAsync(cancellationToken);

    /// <summary>
    /// Устанавливает временные метки создания для новых сущностей.
    /// </summary>
    private void SetTimeStamps()
    {
        var newEntities = ChangeTracker.Entries<IdentityModel>()
            .Where(e => e.State == EntityState.Added);
        foreach (var entity in newEntities)
        {
            if (entity.Property(e => e.CreatedAt).CurrentValue == default)
            {
                entity.Property(e => e.CreatedAt).CurrentValue = DateTime.UtcNow;
            }
        }
    }
}