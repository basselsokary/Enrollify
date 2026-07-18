using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.WriteContext.Context.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Data)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(x => x.OccurredOn)
            .IsRequired();

        builder.Property(x => x.Error)
            .HasColumnType("text");

        builder.HasIndex(x => x.ProcessedOn);

        builder.HasIndex(x => new
        {
            x.ProcessedOn,
            x.OccurredOn
        });
    }
}