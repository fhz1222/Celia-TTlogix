using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.CompanyProfiles;
using FluentValidation;

namespace Application.Validators;

public class AddAddressBookDtoValidator : AbstractValidator<AddAddressBookDto>
{
    public AddAddressBookDtoValidator(IRepository repository)
    {
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
