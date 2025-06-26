using FinanceManager.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManager.CatalogService.EntityFramework.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="RegistryHolder"/> для Entity Framework.
/// </summary>
public class RegistryHolderConfiguration : IEntityTypeConfiguration<RegistryHolder>
{
    /// <summary>
    /// Настраивает свойства и связи сущности <see cref="RegistryHolder"/>.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    public void Configure(EntityTypeBuilder<RegistryHolder> builder)
    {
        builder.HasKey(rh => rh.Id);
        builder.Property(rh => rh.Role).IsRequired();
    }
}