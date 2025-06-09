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
        return await _context.Vehicles
            .AsNoTracking()
            .Where(v => v.Id == id)
            .Select(v => new Vehicle
            {
                Id = v.Id,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Plate = v.Plate
            })
            .FirstOrDefaultAsync();
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
        _context.Vehicles.Update(vehicle);
    }

    public void Remove(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
    }
}
