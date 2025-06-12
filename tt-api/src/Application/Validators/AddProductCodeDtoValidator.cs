using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.AddAreaType;
using Application.UseCases.Registration.Commands.AddProductCode;
using FluentValidation;

namespace Application.Validators;

public class AddProductCodeDtoValidator : AbstractValidator<AddProductCodeDto>
{
    public AddProductCodeDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Name)
            .Must(name => name.Length != 0)
            .WithMessage("ProductCode Name cannot be empty.");
    }
}
