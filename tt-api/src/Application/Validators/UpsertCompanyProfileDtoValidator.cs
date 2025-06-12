using Application.Extensions;
using Application.UseCases.CompanyProfiles;
using FluentValidation;

namespace Application.Validators;

public class UpsertCompanyProfileDtoValidator : AbstractValidator<UpsertCompanyProfileDto>
{
    public UpsertCompanyProfileDtoValidator()
    {
        RuleFor(dto => dto.Code)
            .Must(code => code.IsNotEmpty() && code.Length <= 10)
            .WithMessage("Company Profile code cannot be empty and can be max 10 characters.");

        RuleFor(dto => dto.Name)
            .Must(name => name.IsNotEmpty())
            .WithMessage("Company Profile name cannot be empty.");
    }
}
