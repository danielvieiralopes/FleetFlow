using FleetFlow.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FleetFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            var response = new
            {
                errors = new List<string>(),
                result = new
                {
                    token = result.Token,
                    roles = result.Roles
                }
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { errors = new[] { ex.Message } });
        }
    }
}