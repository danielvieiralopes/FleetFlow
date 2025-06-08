using FleetFlow.Application.Interfaces;
using FleetFlow.Domain.Entities;
using MediatR;
namespace FleetFlow.Application.Features.Vehicles.Queries;
public class GetVehicleByIdQueryHandler : IRequestHandler<GetVehicleByIdQuery, Vehicle?>
{
    private readonly IVehicleRepository _vehicleRepository;
    public GetVehicleByIdQueryHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }
    public async Task<Vehicle?> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        return await _vehicleRepository.GetByIdAsync(request.Id);
    }
}