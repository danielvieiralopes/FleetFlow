using FleetFlow.Application.Interfaces;
using FleetFlow.Domain.Entities;
using MediatR;
namespace FleetFlow.Application.Features.Vehicles.Queries;
public class GetAllVehiclesQueryHandler : IRequestHandler<GetAllVehiclesQuery, IEnumerable<Vehicle>>
{
    private readonly IVehicleRepository _vehicleRepository;

    public GetAllVehiclesQueryHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<IEnumerable<Vehicle>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Plate))
        {
            return await _vehicleRepository.FindByPlateAsync(request.Plate);
        }
        return await _vehicleRepository.GetAllAsync();
    }
}