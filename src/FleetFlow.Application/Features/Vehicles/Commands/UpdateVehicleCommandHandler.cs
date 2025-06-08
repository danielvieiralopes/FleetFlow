using FleetFlow.Application.Interfaces;
using MediatR;
namespace FleetFlow.Application.Features.Vehicles.Commands;
public class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVehicleCommandHandler(IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id);
        if (vehicle is null)
        {
            throw new KeyNotFoundException("Vehicle not found.");
        }

        vehicle.Make = request.Make;
        vehicle.Model = request.Model;
        vehicle.Year = request.Year;
        vehicle.Plate = request.Plate;

        _vehicleRepository.Update(vehicle);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}