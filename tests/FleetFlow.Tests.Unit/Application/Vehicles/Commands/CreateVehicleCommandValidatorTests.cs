using FleetFlow.Application.Features.Vehicles.Commands;
using FluentAssertions;

namespace FleetFlow.Tests.Unit.Application.Vehicles.Commands;

public class CreateVehicleCommandValidatorTests
{
    private readonly CreateVehicleCommandValidator _validator;

    public CreateVehicleCommandValidatorTests()
    {
        _validator = new CreateVehicleCommandValidator();
    }

    [Fact]
    public void Validate_Should_HaveError_WhenYearIsBefore2020()
    {
        // Arrange
        var command = new CreateVehicleCommand("Ford", "Ranger", 2019, "ABC1234");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Year");
    }

    [Fact]
    public void Validate_Should_HaveError_WhenPlateIsTooShort()
    {
        // Arrange
        var command = new CreateVehicleCommand("Ford", "Ranger", 2024, "ABC");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Plate");
    }

    [Fact]
    public void Validate_Should_HaveError_WhenModelIsEmpty()
    {
        // Arrange
        var command = new CreateVehicleCommand("Ford", "", 2024, "ABC1234");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Model");
    }

    [Fact]
    public void Validate_Should_NotHaveError_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateVehicleCommand("Ford", "Ranger", 2024, "ABC1234");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}