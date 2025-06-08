using FleetFlow.Application.Interfaces;
using FleetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FleetFlow.Infrastructure.Persistence.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly FleetFlowDbContext _context;

    public VehicleRepository(FleetFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
    }

    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        return await _context.Vehicles.AsNoTracking().ToListAsync();
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        return await _context.Vehicles.FindAsync(id);
    }

    public async Task<IEnumerable<Vehicle>> FindByPlateAsync(string plate)
    {
        return await _context.Vehicles
            .Where(v => v.Plate.Contains(plate))
            .AsNoTracking()
            .ToListAsync();
    }

    public void Update(Vehicle vehicle)
    {
        // O EF Core rastreia a entidade e a marca como modificada quando SaveChanges for chamado.
        _context.Vehicles.Update(vehicle);
    }

    public void Remove(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
    }
}