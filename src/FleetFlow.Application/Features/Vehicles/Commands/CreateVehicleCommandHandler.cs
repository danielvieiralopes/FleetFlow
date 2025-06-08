using FleetFlow.Application.Interfaces;
using FleetFlow.Domain.Entities;
using MediatR;
namespace FleetFlow.Application.Features.Vehicles.Commands;
public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Guid>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleCommandHandler(IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            Plate = request.Plate
        };
        await _vehicleRepository.AddAsync(vehicle);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return vehicle.Id;
    }
}