using FinanceManager.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManager.CatalogService.EntityFramework.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="Category"/> для Entity Framework.
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    /// <summary>
    /// Настраивает свойства и связи сущности <see cref="Category"/>.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Income).IsRequired();
        builder.Property(c => c.Expense).IsRequired();
        
        builder.HasOne(c => c.RegistryHolder)
            .WithMany()
            .HasForeignKey(c => c.RegistryHolderId)
            .IsRequired();
        
        builder.HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey(c => c.ParentId)
            .IsRequired(false);
    }
}