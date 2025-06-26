using FinanceManager.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManager.CatalogService.EntityFramework.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="Bank"/> для Entity Framework.
/// </summary>
public class BankConfiguration : IEntityTypeConfiguration<Bank>
{
    /// <summary>
    /// Настраивает свойства и связи сущности <see cref="Bank"/>.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    public void Configure(EntityTypeBuilder<Bank> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).IsRequired();
        
        builder.HasOne(b => b.Country)
            .WithMany()
            .HasForeignKey(b => b.CountryId)
            .IsRequired();
    }
}