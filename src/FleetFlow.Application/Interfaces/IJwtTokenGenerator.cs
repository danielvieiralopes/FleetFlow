using FleetFlow.Domain.Entities;

namespace FleetFlow.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}