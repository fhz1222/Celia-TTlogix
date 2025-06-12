using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.AddControlCode;
using FluentValidation;

namespace Application.Validators;

public class AddControlCodeDtoValidator : AbstractValidator<AddControlCodeDto>
{
    public AddControlCodeDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Name)
            .Must(name => name.Length != 0)
            .WithMessage("ControlCode Name cannot be empty.");
    }
}
