using FluentValidation;
namespace FleetFlow.Application.Features.Vehicles.Commands;
public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(v => v.Model).NotEmpty();
        RuleFor(v => v.Year).GreaterThanOrEqualTo(2020);
        RuleFor(v => v.Plate).NotEmpty().MinimumLength(7);
    }
}