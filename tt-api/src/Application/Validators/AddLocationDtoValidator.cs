using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.AddLocation;
using FluentValidation;

namespace Application.Validators;

public class AddLocationDtoValidator : AbstractValidator<AddLocationDto>
{
    public AddLocationDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Code)
            .Must(code => code.Length <= 15)
            .WithMessage("Location Code is max 15 characters.");

        RuleFor(dto => dto.Code)
            .Must(code => code.Length != 0)
            .WithMessage("Location Code cannot be empty.");

        RuleFor(dto => dto.Name)
            .Must(name => name.Length != 0)
            .WithMessage("Location Name cannot be empty.");

        RuleFor(dto => dto)
            .Must(dto => repository.Utils.CheckIfAreaExists(dto.AreaCode, dto.WarehouseCode))
            .WithMessage("Location Area must be valid.");

        RuleFor(dto => dto.WarehouseCode)
            .Must(whsCode => repository.Utils.CheckIfWhsCodeExists(whsCode))
            .WithMessage("Location Warehouse Code must be valid.");
    }
}
