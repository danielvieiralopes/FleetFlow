using FleetFlow.Application.Features.Vehicles.Commands;
using FleetFlow.Application.Interfaces;
using FleetFlow.Domain.Entities;
using Moq;
using FluentAssertions;

namespace FleetFlow.Tests.Unit.Application.Vehicles.Commands;

public class CreateVehicleCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateVehicleCommandHandler _handler;

    public CreateVehicleCommandHandlerTests()
    {
        // ARRANGE - Configuração inicial para todos os testes nesta classe
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateVehicleCommandHandler(_mockVehicleRepository.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Should_AddVehicleAndSaveChanges_WhenCommandIsValid()
    {
        // ARRANGE - Configuração específica para este teste
        // 1. Cria o comando com dados de exemplo.
        var command = new CreateVehicleCommand("Ford", "Ranger", 2024, "ABC1234");

        // ACT - Executa a ação que estamos a testar
        // 2. Chama o método Handle do nosso command handler.
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT - Verifica se os resultados são os esperados
        // 3. Garante que o ID do veículo retornado não é um Guid vazio.
        result.Should().NotBeEmpty();

        // 4. Verifica se o método AddAsync do repositório foi chamado exatamente uma vez
        // com um objeto Vehicle. O It.IsAny<Vehicle>() significa que não nos importamos
        // com os detalhes do veículo, apenas que um veículo foi passado.
        _mockVehicleRepository.Verify(repo =>
            repo.AddAsync(It.IsAny<Vehicle>()),
            Times.Once);

        // 5. Verifica se o método SaveChangesAsync da Unit of Work foi chamado exatamente uma vez.
        // Isto é crucial para garantir que os dados são persistidos no banco.
        _mockUnitOfWork.Verify(uow =>
            uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}