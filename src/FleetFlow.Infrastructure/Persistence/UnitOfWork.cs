using FleetFlow.Application.Interfaces;

namespace FleetFlow.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly FleetFlowDbContext _context;

    public UnitOfWork(FleetFlowDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}