using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.CompanyProfiles;
using FluentValidation;

namespace Application.Validators;

public class UpdateAddressBookDtoValidator : AbstractValidator<UpdateAddressBookDto>
{
    public UpdateAddressBookDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Code)
            .Must(code => code.IsNotEmpty() && code.Length <= 12)
            .WithMessage("Address Book code cannot be empty and can be max 12 characters.");

        RuleFor(dto => dto)
            .MustNot(dto => dto.Address1.IsEmpty() && dto.Address2.IsEmpty() && dto.Address3.IsEmpty() && dto.Address4.IsEmpty())
            .WithMessage("Address Book needs at least one address.");

        RuleFor(dto => dto.CompanyCode)
            .MustNot(companyCode => repository.CompanyProfiles.CompanyProfileDoesNotExistsOrIsInactive(companyCode))
            .WithMessage((_, companyCode) => $"Invalid or inactive company profile {companyCode}.");

        RuleFor(dto => dto.Country)
            .MustNot(country => repository.Countries.CountryDoesNotExistsOrIsInactive(country))
            .WithMessage((_, country) => $"Invalid country code {country}.");
    }
}
