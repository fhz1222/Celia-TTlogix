using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.AddArea;
using FluentValidation;

namespace Application.Validators;

public class AddAreaDtoValidator : AbstractValidator<AddAreaDto>
{
    public AddAreaDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Code)
            .Must(code => code.Length <= 7)
            .WithMessage("Area Code is max 7 characters.");

        RuleFor(dto => dto.Code)
            .Must(code => code.Length != 0)
            .WithMessage("Area Code cannot be empty.");

        RuleFor(dto => dto.Name)
            .Must(name => name.Length != 0)
            .WithMessage("Area Name cannot be empty.");

        RuleFor(dto => dto.Type)
            .Must(type => repository.Utils.CheckIfAreaTypeExists(type))
            .WithMessage("Area Type must be valid.");

        RuleFor(dto => dto.WhsCode)
            .Must(whsCode => repository.Utils.CheckIfWhsCodeExists(whsCode))
            .WithMessage("Area Warehouse Code must be valid.");
    }
}
