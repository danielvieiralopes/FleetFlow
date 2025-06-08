using MediatR;
namespace FleetFlow.Application.Features.Vehicles.Commands;
public record DeleteVehicleCommand(Guid Id) : IRequest;