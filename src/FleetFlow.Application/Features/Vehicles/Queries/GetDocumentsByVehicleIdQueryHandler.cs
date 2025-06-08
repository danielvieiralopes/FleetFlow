using FleetFlow.Application.Interfaces;
using FleetFlow.Domain.Entities;
using MediatR;
namespace FleetFlow.Application.Features.Vehicles.Queries;
public class GetDocumentsByVehicleIdQueryHandler : IRequestHandler<GetDocumentsByVehicleIdQuery, IEnumerable<Document>>
{
    private readonly IDocumentRepository _documentRepository;

    public GetDocumentsByVehicleIdQueryHandler(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<IEnumerable<Document>> Handle(GetDocumentsByVehicleIdQuery request, CancellationToken cancellationToken)
    {
        return await _documentRepository.GetByVehicleIdAsync(request.VehicleId);
    }
}