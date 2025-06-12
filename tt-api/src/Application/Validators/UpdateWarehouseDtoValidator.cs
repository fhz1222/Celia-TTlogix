using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.UpdateWarehouse;
using FluentValidation;

namespace Application.Validators;

public class UpdateWarehouseDtoValidator : AbstractValidator<UpdateWarehouseDto>
{
    public UpdateWarehouseDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Country)
            .Must(code => code.Length != 0)
            .WithMessage("Warehouse country cannot be empty.");

        RuleFor(dto => dto.Country)
            .MustNot(country => repository.Countries.CountryDoesNotExistsOrIsInactive(country))
            .WithMessage((_, country) => $"Invalid country code {country}.");
    }
}
