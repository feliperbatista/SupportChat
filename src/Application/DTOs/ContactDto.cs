using System;

namespace Application.DTOs;

public record ContactDto
(
    Guid Id,
    string PhoneNumber,
    string Name,
    string? ProfilePictureUrl
);