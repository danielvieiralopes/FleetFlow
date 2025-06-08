using FleetFlow.Application.Interfaces;
using FleetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FleetFlow.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementação concreta do repositório de usuários usando Entity Framework Core.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly FleetFlowDbContext _context;

    public UserRepository(FleetFlowDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        // Busca o primeiro usuário que corresponda ao nome de usuário, de forma assíncrona.
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        // Adiciona um novo usuário ao contexto do EF Core.
        await _context.Users.AddAsync(user);
        // Nota: A chamada para SaveChangesAsync() será feita em outro lugar (ex: Unit of Work)
        // para garantir que múltiplas operações possam ser salvas em uma única transação.
    }
}