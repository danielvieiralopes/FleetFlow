using MediatR;
using FleetFlow.Domain.Entities;
namespace FleetFlow.Application.Features.Vehicles.Queries;
public record GetAllVehiclesQuery(string? Plate) : IRequest<IEnumerable<Vehicle>>;