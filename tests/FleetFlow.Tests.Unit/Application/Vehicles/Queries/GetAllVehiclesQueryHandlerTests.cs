using FleetFlow.Application.Features.Vehicles.Queries;
using FleetFlow.Application.Interfaces;
using FleetFlow.Domain.Entities;
using Moq;
using FluentAssertions;

namespace FleetFlow.Tests.Unit.Application.Vehicles.Queries;

public class GetAllVehiclesQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly GetAllVehiclesQueryHandler _handler;

    public GetAllVehiclesQueryHandlerTests()
    {
        // Arrange
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _handler = new GetAllVehiclesQueryHandler(_mockVehicleRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnAllVehicles_WhenPlateIsNull()
    {
        // Arrange
        var vehicles = new List<Vehicle>
        {
            new() { Id = Guid.NewGuid(), Plate = "ABC1111" },
            new() { Id = Guid.NewGuid(), Plate = "DEF2222" }
        };

        // Configura o mock para retornar a lista de veículos quando GetAllAsync for chamado.
        _mockVehicleRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(vehicles);

        var query = new GetAllVehiclesQuery(null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(vehicles);
        _mockVehicleRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        _mockVehicleRepository.Verify(repo => repo.FindByPlateAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnVehiclesByPlate_WhenPlateIsProvided()
    {
        // Arrange
        var plate = "XYZ";
        var vehicles = new List<Vehicle>
        {
            new() { Id = Guid.NewGuid(), Plate = "XYZ7777" }
        };

        // Configura o mock para retornar a lista filtrada quando FindByPlateAsync for chamado.
        _mockVehicleRepository.Setup(repo => repo.FindByPlateAsync(plate)).ReturnsAsync(vehicles);

        var query = new GetAllVehiclesQuery(plate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(vehicles);
        _mockVehicleRepository.Verify(repo => repo.FindByPlateAsync(plate), Times.Once);
        _mockVehicleRepository.Verify(repo => repo.GetAllAsync(), Times.Never);
    }
}