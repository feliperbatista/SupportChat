using System;

namespace Domain.Enums;

public enum MessageType
{
    Text,
    Image,
    Audio,
    Video,
    Document,
    Sticker,
    Location,
    Reaction,
    Unknown
}
