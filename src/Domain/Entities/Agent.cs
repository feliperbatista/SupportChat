using System;
using Domain.Enums;

namespace Domain.Entities;

public class Agent
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; } = string.Empty;
    public AgentStatus Status { get; private set; } = AgentStatus.Offline;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    public AgentRole Role { get; private set; } = AgentRole.User;

    public ICollection<Conversation> Conversations { get; private set; } = [];

    private Agent() {}

    public static Agent Create(string name, string email, string passwordHash)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrEmpty(passwordHash))
            throw new ArgumentException("Password hash is required", nameof(passwordHash));

        return new Agent
        {
            Name = name,
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash
        };
    }

    public void UpdateStatus(AgentStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string name, string? avatarUrl)
    {
        Name = name;
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        Status = AgentStatus.Offline;
        UpdatedAt = DateTime.UtcNow;
    }
}
