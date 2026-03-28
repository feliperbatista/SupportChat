using System;

namespace Infrastructure.WhatsApp;

public class WhatsAppOptions
{
    public string Token { get; set; } = string.Empty;
    public string PhoneNumberId { get; set; } = string.Empty;
    public string VerifyToken { get; set; } = string.Empty;
}
