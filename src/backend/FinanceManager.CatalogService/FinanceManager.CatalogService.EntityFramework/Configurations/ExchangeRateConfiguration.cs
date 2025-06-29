using FinanceManager.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceManager.CatalogService.EntityFramework.Configurations;

/// <summary>
/// Конфигурация сущности <see cref="ExchangeRate"/> для Entity Framework.
/// </summary>
public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
{
    /// <summary>
    /// Настраивает свойства и связи сущности <see cref="ExchangeRate"/>.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    public void Configure(EntityTypeBuilder<ExchangeRate> builder)
    {
        builder.HasKey(er => er.Id);
        builder.Property(er => er.RateDate).IsRequired();
        builder.Property(er => er.Rate).IsRequired();
        
        builder.HasOne(er => er.Currency)
            .WithMany()
            .HasForeignKey(er => er.CurrencyId)
            .IsRequired();
    }
}