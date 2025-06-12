using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.UpdateArea;
using FluentValidation;

namespace Application.Validators;

public class UpdateAreaDtoValidator : AbstractValidator<UpdateAreaDto>
{
    public UpdateAreaDtoValidator(IRepository repository)
    {
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
