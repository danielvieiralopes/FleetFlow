using MediatR;
namespace FleetFlow.Application.Features.Vehicles.Commands;
public record UpdateVehicleCommand(Guid Id, string Make, string Model, int Year, string Plate) : IRequest;