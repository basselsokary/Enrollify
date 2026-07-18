using Domain.Entities.PaymentAggregate;
using Infrastructure.Persistence.WriteContext.Context.Configurations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.WriteContext.Context.Configurations.Entities;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.EnrollmentId)
            .IsRequired();

        builder.Property(x => x.PaymentIntentId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.AmountRefunded)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(x => x.PaidAt)
            .IsRequired(false);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.EnrollmentId)
            .IsUnique();
        builder.HasIndex(x => x.PaymentIntentId)
            .IsUnique();
        builder.HasIndex(x => x.Status);

        builder.OwnsOne(x => x.Money, money => money.Configure("Money"));
    }
}