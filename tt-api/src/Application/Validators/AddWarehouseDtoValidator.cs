using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.AddWarehouse;
using FluentValidation;

namespace Application.Validators;

public class AddWarehouseDtoValidator : AbstractValidator<AddWarehouseDto>
{
    public AddWarehouseDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Code)
            .Must(code => code.Length <= 7)
            .WithMessage("Warehouse Code is max 7 characters.");

        RuleFor(dto => dto.Code)
            .Must(code => code.Length != 0)
            .WithMessage("Warehouse Code cannot be empty.");

        RuleFor(dto => dto.Code)
            .MustNot(code => repository.Utils.CheckIfWhsCodeExists(code))
            .WithMessage((_, companyCode) => $"Warehouse with this code already exists.");

        RuleFor(dto => dto.Country)
            .Must(code => code.Length != 0)
            .WithMessage("Warehouse country cannot be empty.");

        RuleFor(dto => dto.Country)
            .MustNot(country => repository.Countries.CountryDoesNotExistsOrIsInactive(country))
            .WithMessage((_, country) => $"Invalid country code {country}.");
    }
}
