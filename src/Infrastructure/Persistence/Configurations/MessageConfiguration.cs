using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.WhatsAppMessageId)
            .HasMaxLength(100);

        builder.HasIndex(m => m.WhatsAppMessageId)
            .IsUnique()
            .HasFilter("[WhatsAppMessageId] IS NOT NULL");

        builder.Property(m => m.Content)
            .HasMaxLength(4096);

        builder.Property(m => m.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.MediaUrl)
            .HasMaxLength(1000);

        builder.Property(m => m.MediaId)
            .HasMaxLength(200);

        builder.Property(m => m.MediaMimeType)
            .HasMaxLength(100);

        builder.HasMany(m => m.Reactions)
            .WithOne(r => r.Message)
            .HasForeignKey(r => r.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.SentByAgent)
            .WithMany()
            .HasForeignKey(m => m.SentByAgentId);

        builder.HasIndex(m => m.ConversationId);
        builder.HasIndex(m => m.CreatedAt);
    }
}
