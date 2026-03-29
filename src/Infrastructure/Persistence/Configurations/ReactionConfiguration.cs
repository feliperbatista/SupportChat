using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Emoji)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(r => r.FromPhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(r => new { r.MessageId, r.FromPhoneNumber, r.Emoji })
            .IsUnique()
            .HasDatabaseName("IX_Reactions_MessageId_From_Emoji");
    }
}
