using System;

namespace Domain.Entities;

public class Reaction
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid MessageId { get; private set; }
    public Message Message { get; private set; } = null!;
    public string Emoji { get; private set; } = string.Empty;
    public string FromPhoneNumber { get; private set; } = string.Empty;
    public bool IsFromAgent { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Reaction() {}

    public static Reaction Create(
        Guid messageId,
        string emoji,
        string fromPhoneNumber,
        bool isFromAgent
    )
    {
        return new Reaction
        {
            MessageId = messageId,
            Emoji = emoji,
            FromPhoneNumber = fromPhoneNumber,
            IsFromAgent = isFromAgent
        };
    }
}
