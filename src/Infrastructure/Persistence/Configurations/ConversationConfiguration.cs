using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(c => c.Contact)
            .WithMany(c => c.Conversations)
            .HasForeignKey(c => c.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.AssignedAgent)
            .WithMany(a => a.Conversations)
            .HasForeignKey(c => c.AssignedAgentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Departments)
            .WithMany(d => d.Conversations)
            .UsingEntity(j => j.ToTable("ConversationDepartments"));

        builder.HasIndex(c => c.AssignedAgentId);
        builder.HasIndex(c => c.Status);
    }
}
