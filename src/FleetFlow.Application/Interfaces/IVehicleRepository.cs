using FleetFlow.Domain.Entities;

namespace FleetFlow.Application.Interfaces
{
    /// <summary>
    /// Define o contrato para o repositório de veículos.
    /// </summary>
    public interface IVehicleRepository
    {
        Task<Vehicle?> GetByIdAsync(Guid id);
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<IEnumerable<Vehicle>> FindByPlateAsync(string plate);
        Task AddAsync(Vehicle vehicle);
        void Update(Vehicle vehicle);
        void Remove(Vehicle vehicle);
    }
}
