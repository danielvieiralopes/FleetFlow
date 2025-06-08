using MediatR;
using FleetFlow.Domain.Entities;
namespace FleetFlow.Application.Features.Vehicles.Queries;
public record GetVehicleByIdQuery(Guid Id) : IRequest<Vehicle?>;