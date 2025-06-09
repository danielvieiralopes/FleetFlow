using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FleetFlow.Application.Features.Vehicles.Commands;
using FleetFlow.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FleetFlow.Tests.Integration.Vehicles.Commands;

/// <summary>
/// Testa o ciclo de vida completo de um veículo (GET, PUT, DELETE) através de testes de integração.
/// </summary>
public class VehicleLifecycleTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public VehicleLifecycleTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        // Garante que todos os pedidos deste cliente são "autenticados" com o nosso esquema de teste.
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
    }

    [Fact]
    public async Task GetVehicle_ShouldReturnOkAndVehicle_WhenVehicleExists()
    {
        // Arrange
        // 1. Cria um veículo de teste diretamente na base de dados.
        var vehicle = new Vehicle { Id = Guid.NewGuid(), Model = "Cybertruck", Plate = "TSL2025", Year = 2025 };
        await AddVehicleToDatabaseAsync(vehicle);

        // Act
        // 2. Faz um pedido GET para o endpoint com o ID do veículo.
        var response = await _client.GetAsync($"/api/vehicles/{vehicle.Id}");

        // Assert
        // 3. Verifica se a resposta foi bem-sucedida e se o conteúdo está correto.
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseString = await response.Content.ReadAsStringAsync();
        var vehicleFromApi = JsonSerializer.Deserialize<SingleVehicleResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        vehicleFromApi.Should().NotBeNull();
        vehicleFromApi?.result.Plate.Should().Be("TSL2025");
    }

    [Fact]
    public async Task UpdateVehicle_ShouldUpdateDatabase_WhenDataIsValid()
    {
        // Arrange
        // 1. Cria um veículo de teste para ser atualizado.
        var vehicle = new Vehicle { Id = Guid.NewGuid(), Model = "Roadster", Plate = "TSL2026", Year = 2026 };
        await AddVehicleToDatabaseAsync(vehicle);
        var updateCommand = new UpdateVehicleCommand(vehicle.Id, "Tesla", "Roadster 2.0", 2027, "TSL2027");

        // Act
        // 2. Faz um pedido PUT para o endpoint.
        var response = await _client.PutAsJsonAsync($"/api/vehicles/{vehicle.Id}", updateCommand);

        // Assert
        // 3. Verifica se a resposta foi bem-sucedida.
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // 4. Verifica diretamente na base de dados se os dados foram realmente atualizados.
        var vehicleInDb = await FindVehicleInDatabaseAsync(vehicle.Id);
        vehicleInDb?.Model.Should().Be("Roadster 2.0");
        vehicleInDb?.Year.Should().Be(2027);
    }

    [Fact]
    public async Task DeleteVehicle_ShouldRemoveFromDatabase_WhenVehicleExists()
    {
        // Arrange
        // 1. Cria um veículo de teste para ser eliminado.
        var vehicle = new Vehicle { Id = Guid.NewGuid(), Model = "Semi", Plate = "TSL2028", Year = 2028 };
        await AddVehicleToDatabaseAsync(vehicle);

        // Act
        // 2. Faz um pedido DELETE para o endpoint.
        var response = await _client.DeleteAsync($"/api/vehicles/{vehicle.Id}");

        // Assert
        // 3. Verifica se a resposta foi "No Content", indicando sucesso.
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 4. Verifica na base de dados se o veículo foi realmente removido.
        var vehicleInDb = await FindVehicleInDatabaseAsync(vehicle.Id);
        vehicleInDb.Should().BeNull();
    }

    // --- MÉTODOS AUXILIARES PARA MANTER OS TESTES LIMPOS ---

    private async Task AddVehicleToDatabaseAsync(Vehicle vehicle)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FleetFlow.Infrastructure.Persistence.FleetFlowDbContext>();
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();
        // Desanexa a entidade para evitar problemas de rastreamento de estado do EF Core.
        context.Entry(vehicle).State = EntityState.Detached;
    }

    private async Task<Vehicle?> FindVehicleInDatabaseAsync(Guid id)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FleetFlow.Infrastructure.Persistence.FleetFlowDbContext>();
        return await context.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
    }

    // DTO para deserializar a resposta do GET, que tem um formato específico.
    public record SingleVehicleResponse(List<string> errors, Vehicle result);
}
