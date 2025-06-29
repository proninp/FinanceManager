using FinanceManager.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManager.CatalogService.EntityFramework.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="Account"/> для Entity Framework.
/// </summary>
public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    /// <summary>
    /// Настраивает свойства и связи сущности <see cref="Account"/>.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).IsRequired();
        builder.Property(a => a.IsIncludeInBalance).IsRequired();
        builder.Property(a => a.IsDefault).IsRequired();
        builder.Property(a => a.IsArchived).IsRequired();
        builder.Property(a => a.IsDeleted).IsRequired();
        
        builder.HasOne(a => a.RegistryHolder)
            .WithMany()
            .HasForeignKey(a => a.RegistryHolderId)
            .IsRequired();
        
        builder.HasOne(a => a.AccountType)
            .WithMany()
            .HasForeignKey(a => a.AccountTypeId)
            .IsRequired();
        
        builder.HasOne(a => a.Currency)
            .WithMany()
            .HasForeignKey(a => a.CurrencyId)
            .IsRequired();
        
        builder.HasOne(a => a.Bank)
            .WithMany()
            .HasForeignKey(a => a.BankId)
            .IsRequired();
    }
}