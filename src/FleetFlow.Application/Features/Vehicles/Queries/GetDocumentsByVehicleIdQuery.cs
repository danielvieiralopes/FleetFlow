using MediatR;
using FleetFlow.Domain.Entities;
namespace FleetFlow.Application.Features.Vehicles.Queries;
public record GetDocumentsByVehicleIdQuery(Guid VehicleId) : IRequest<IEnumerable<Document>>;