using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.WriteContext.Context.Configurations.ValueObjects;

internal static class MoneyConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, Money> ownerBuilder,
        string prefix = "Price") where T : class
    {
        ownerBuilder.Property(m => m.Amount)
            .HasColumnName($"{prefix}_Amount")
            .HasPrecision(Money.Precision, Money.Scale);

        ownerBuilder.Property(m => m.Currency)
            .HasColumnName($"{prefix}_Currency")
            .HasMaxLength(Money.MaxCurrencyLength);
    }
}