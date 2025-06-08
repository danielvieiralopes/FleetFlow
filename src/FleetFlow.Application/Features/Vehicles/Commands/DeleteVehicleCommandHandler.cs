using FleetFlow.Application.Interfaces;
using MediatR;
namespace FleetFlow.Application.Features.Vehicles.Commands;
public class DeleteVehicleCommandHandler : IRequestHandler<DeleteVehicleCommand>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteVehicleCommandHandler(IVehicleRepository vehicleRepository, IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.Id);
        if (vehicle is not null)
        {
            _vehicleRepository.Remove(vehicle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}