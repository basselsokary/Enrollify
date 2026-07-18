using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Common.Constants;
using Infrastructure.Identity;

namespace Infrastructure.Persistence.WriteContext.Context.Configurations;

internal sealed class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.Property(user => user.FullName)
            .HasMaxLength(DomainConstants.User.MaxFullNameLength)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(DomainConstants.User.MaxEmailLength)
            .IsRequired();
        
        builder.Property(user => user.CreatedAt)
            .IsRequired();
    }
}
