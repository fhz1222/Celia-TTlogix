using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.AddPackageType;
using FluentValidation;

namespace Application.Validators;

public class AddPackageTypeDtoValidator : AbstractValidator<AddPackageTypeDto>
{
    public AddPackageTypeDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Name)
            .Must(name => name.Length != 0)
            .WithMessage("PackageType Name cannot be empty.");
    }
}
