using System;

namespace Domain.Entities;

public class Contact
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? ProfilePictureUrl { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public ICollection<Conversation> Conversations { get; private set; } = [];

    private Contact() {}

    public static Contact Create(string phoneNumber, string name)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            throw new ArgumentException("Phone number is requried", nameof(phoneNumber));

        return new Contact
        {
            PhoneNumber = phoneNumber,
            Name = string.IsNullOrEmpty(name) ? phoneNumber : name
        };
    }

    public void UpdateName(string name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfilePicture(string url)
    {
        ProfilePictureUrl = url;
        UpdatedAt = DateTime.UtcNow;
    }
}
