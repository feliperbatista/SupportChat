using System;
using Domain.Enums;

namespace Domain.Entities;

public class Conversation
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ContactId { get; private set; }
    public Contact Contact { get; private set; } = null!;
    public Guid? AssignedAgentId { get; private set; }
    public Agent? AssignedAgent { get; private set; }
    public DateTime? AssignedAt { get; private set; }
    public ConversationStatus Status { get; private set; } = ConversationStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    public ICollection<Department> Departments { get; private set; } = [];
    public ICollection<Message> Messages { get; private set; } = [];

    private Conversation() {}

    public static Conversation Create(Guid contactId)
    {
        return new Conversation { ContactId = contactId };
    }

    public void AssignTo(Guid agentId)
    {
        AssignedAgentId = agentId;
        AssignedAt = DateTime.UtcNow;
        Status = ConversationStatus.Open;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unassign()
    {
        AssignedAgentId = null;
        AssignedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resolve()
    {
        Status = ConversationStatus.Resolved;
        ClosedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reopen()
    {
        Status = ConversationStatus.Open;
        ClosedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsInQueue() => AssignedAgentId is null;
}
