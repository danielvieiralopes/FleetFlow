using System.Net;
using System.Net.Http.Json;
using FleetFlow.Application.Features.Vehicles.Commands;
using FleetFlow.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FleetFlow.Tests.Integration.Vehicles.Commands;

/// <summary>
/// Classe de teste de integração para a criação de veículos.
/// IClassFixture<T> informa ao xUnit para criar uma única instância da nossa fábrica para todos os testes nesta classe.
/// </summary>
public class CreateVehicleTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public CreateVehicleTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateVehicle_Should_CreateVehicleInDatabase_WhenDataIsValid()
    {
        // ARRANGE
        // 1. O ambiente já está preparado pela nossa factory (API e DB em contêineres).
        var command = new CreateVehicleCommand("Tesla", "Model Y", 2024, "TSL2024");

        // Simular autenticação (para um teste real, obteríamos um token JWT real)
        // Por simplicidade aqui, vamos pular a parte do token, pois o foco é a integração com o DB.
        // Em um cenário completo, você faria um POST em /api/auth/login e usaria o token.
        // _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your_jwt_token");


        // ACT
        // 2. Fazemos uma requisição HTTP POST para o nosso endpoint.
        var response = await _client.PostAsJsonAsync("/api/vehicles", command);


        // ASSERT
        // 3. Verificamos se a resposta da API foi bem-sucedida (201 Created).
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // 4. Verificamos se o veículo foi realmente salvo no banco de dados de teste.
        // Criamos um novo escopo de serviço para obter uma instância do nosso DbContext.
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FleetFlow.Infrastructure.Persistence.FleetFlowDbContext>();

        // Buscamos no banco de dados de teste por um veículo com a placa que acabamos de criar.
        var vehicleInDb = await dbContext.Vehicles.FirstOrDefaultAsync(v => v.Plate == "TSL2024");

        // Garantimos que o veículo não é nulo e que seus dados correspondem ao que enviamos.
        vehicleInDb.Should().NotBeNull();
        vehicleInDb?.Model.Should().Be("Model Y");
        vehicleInDb?.Year.Should().Be(2024);
    }
}
