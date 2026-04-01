using System;

namespace Application.DTOs;

public record CategoryDto(
    Guid Id,
    string Name, 
    Guid DeparmentId
);
