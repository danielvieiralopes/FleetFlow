using FleetFlow.Application.Features.Vehicles.Commands;
using FleetFlow.Application.Features.Vehicles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FleetFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VehiclesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "vehicle-admin")]
    public async Task<IActionResult> CreateVehicle(CreateVehicleCommand command)
    {
        var vehicleId = await _mediator.Send(command);
        var vehicle = new { id = vehicleId, command.Make, command.Model, command.Year, command.Plate };
        return CreatedAtAction(nameof(GetVehicleById), new { id = vehicleId }, new { errors = new List<string>(), result = vehicle });
    }

    [HttpGet]
    [Authorize(Roles = "vehicle-read")] 
    public async Task<IActionResult> GetAllVehicles([FromQuery] string? plate)
    {
        var query = new GetAllVehiclesQuery(plate);
        var vehicles = await _mediator.Send(query);
        return Ok(new { errors = new List<string>(), result = vehicles });
    }


    [HttpGet("{id:guid}")]
    [Authorize(Roles = "vehicle-read")]
    public async Task<IActionResult> GetVehicleById(Guid id)
    {
        var vehicle = await _mediator.Send(new GetVehicleByIdQuery(id));
        if (vehicle is null)
        {
            return NotFound(new { errors = new[] { "Vehicle not found." } });
        }
        return Ok(new { errors = new List<string>(), result = vehicle });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "vehicle-admin")]
    public async Task<IActionResult> UpdateVehicle(Guid id, UpdateVehicleCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { errors = new[] { "Route ID does not match command ID." } });
        }

        try
        {
            await _mediator.Send(command);
            var updatedVehicle = await _mediator.Send(new GetVehicleByIdQuery(id));
            return Ok(new { errors = new List<string>(), result = updatedVehicle });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { errors = new[] { ex.Message } });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "vehicle-admin")]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        var command = new DeleteVehicleCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{id:guid}/file")]
    [Authorize(Roles = "vehicle-admin")]
    public async Task<IActionResult> RequestUpload(Guid id, [FromBody] RequestUploadPayload payload)
    {
        var command = new RequestUploadCommand(id, payload.FileName, payload.FileMimeType);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    public record RequestUploadPayload(string FileName, string FileMimeType);

    [HttpGet("{id:guid}/file")]
    [Authorize(Roles = "vehicle-read")]
    public async Task<IActionResult> GetVehicleFiles(Guid id)
    {
        var documents = await _mediator.Send(new GetDocumentsByVehicleIdQuery(id));
        return Ok(new { errors = new List<string>(), result = documents });
    }
}
