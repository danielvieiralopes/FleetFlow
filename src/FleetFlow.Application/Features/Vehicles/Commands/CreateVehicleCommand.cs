using MediatR;
namespace FleetFlow.Application.Features.Vehicles.Commands;
public record CreateVehicleCommand(string Make, string Model, int Year, string Plate) : IRequest<Guid>;