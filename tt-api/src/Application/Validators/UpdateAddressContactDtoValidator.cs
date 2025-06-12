using Application.Extensions;
using Application.UseCases.CompanyProfiles;
using FluentValidation;

namespace Application.Validators;

public class UpdateAddressContactDtoValidator : AbstractValidator<UpdateAddressContactDto>
{
    public UpdateAddressContactDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .Must(name => name.IsNotEmpty())
            .WithMessage("Address Contact name cannot be empty.");
    }
}