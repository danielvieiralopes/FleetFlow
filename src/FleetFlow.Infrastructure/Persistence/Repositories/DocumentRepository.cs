using FleetFlow.Application.Interfaces;
using FleetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace FleetFlow.Infrastructure.Persistence.Repositories;
public class DocumentRepository : IDocumentRepository
{
    private readonly FleetFlowDbContext _context;
    public DocumentRepository(FleetFlowDbContext context) { _context = context; }
    public async Task AddAsync(Document document) => await _context.Documents.AddAsync(document);
    public async Task<IEnumerable<Document>> GetByVehicleIdAsync(Guid vehicleId)
    {
        return await _context.Documents
            .Where(d => d.VehicleId == vehicleId)
            .AsNoTracking()
            .ToListAsync();
    }
}