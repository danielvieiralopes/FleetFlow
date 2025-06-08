using FleetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FleetFlow.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(FleetFlowDbContext context)
    {
        // Garante que o banco de dados seja criado
        await context.Database.MigrateAsync();

        // Adiciona usuários apenas se a tabela estiver vazia
        if (!await context.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new() {
                    Id = Guid.NewGuid(),
                    Username = "admin@acme.com",
                    PasswordHash = "admin123", // Em um projeto real, isso seria um HASH
                    Roles = new[] { "vehicle-read", "vehicle-admin" }
                },
                new() {
                    Id = Guid.NewGuid(),
                    Username = "analista@acme.com",
                    PasswordHash = "analista123", // Em um projeto real, isso seria um HASH
                    Roles = new[] { "vehicle-read" }
                }
            };
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}