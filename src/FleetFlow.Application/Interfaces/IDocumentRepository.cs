using FleetFlow.Domain.Entities;
namespace FleetFlow.Application.Interfaces;
public interface IDocumentRepository
{
    Task AddAsync(Document document);
    Task<IEnumerable<Document>> GetByVehicleIdAsync(Guid vehicleId);
}