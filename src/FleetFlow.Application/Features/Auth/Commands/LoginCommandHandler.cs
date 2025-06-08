using FleetFlow.Application.Interfaces;
using MediatR;
using System.Security.Authentication;

namespace FleetFlow.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        // ATENÇÃO: Verificação de senha simplificada. Em um projeto real, use BCrypt, Argon2, etc.
        // Aqui, estamos comparando a senha do comando com a senha (que será o hash) do banco.
        if (user is null || user.PasswordHash != request.Password)
        {
            throw new AuthenticationException("Invalid username or password.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new LoginResult(token, user.Roles.ToList());
    }
}