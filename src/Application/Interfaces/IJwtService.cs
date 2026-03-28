using System;
using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Agent agent);
    Guid? ValidateToken(string token);
}
