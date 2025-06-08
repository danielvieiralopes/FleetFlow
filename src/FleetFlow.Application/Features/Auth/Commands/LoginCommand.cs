using MediatR;

namespace FleetFlow.Application.Features.Auth.Commands;

public record LoginCommand(string Username, string Password) : IRequest<LoginResult>;

public record LoginResult(string Token, List<string> Roles);