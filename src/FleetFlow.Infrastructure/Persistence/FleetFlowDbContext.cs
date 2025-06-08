using FleetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FleetFlow.Infrastructure.Persistence;

/// <summary>
/// Representa a sessão com o banco de dados da aplicação.
/// </summary>
public class FleetFlowDbContext : DbContext
{
    public FleetFlowDbContext(DbContextOptions<FleetFlowDbContext> options) : base(options)
    {
    }


    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FleetFlowDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}