using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.AddAreaType;
using FluentValidation;

namespace Application.Validators;

public class AddAreaTypeDtoValidator : AbstractValidator<AddAreaTypeDto>
{
    public AddAreaTypeDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Code)
            .Must(code => code.Length <= 7)
            .WithMessage("AreaType Code is max 7 characters.");

        RuleFor(dto => dto.Code)
            .Must(code => code.Length != 0)
            .WithMessage("AreaType Code cannot be empty.");

        RuleFor(dto => dto.Name)
            .Must(name => name.Length != 0)
            .WithMessage("AreaType Name cannot be empty.");
    }
}
