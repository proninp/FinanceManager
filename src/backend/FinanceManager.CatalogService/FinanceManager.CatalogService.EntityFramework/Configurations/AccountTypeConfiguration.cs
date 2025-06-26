using FinanceManager.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManager.CatalogService.EntityFramework.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="AccountType"/> для Entity Framework.
/// </summary>
public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
{
    /// <summary>
    /// Настраивает свойства и связи сущности <see cref="AccountType"/>.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    public void Configure(EntityTypeBuilder<AccountType> builder)
    {
        builder.HasKey(ac => ac.Id);
        builder.Property(ac => ac.Code).IsRequired();
        builder.Property(ac => ac.Description).IsRequired();
        builder.Property(ac => ac.IsDeleted).IsRequired();
    }
}