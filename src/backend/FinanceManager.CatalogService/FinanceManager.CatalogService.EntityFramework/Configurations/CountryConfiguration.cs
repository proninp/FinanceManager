using FinanceManager.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManager.CatalogService.EntityFramework.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="Country"/> для Entity Framework.
/// </summary>
public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    /// <summary>
    /// Настраивает свойства и связи сущности <see cref="Country"/>.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired();
    }
}