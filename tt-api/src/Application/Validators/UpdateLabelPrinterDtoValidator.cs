using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.UpdateLabelPrinter;
using FluentValidation;

namespace Application.Validators;

public class UpdateLabelPrinterDtoValidator : AbstractValidator<UpdateLabelPrinterDto>
{
    public UpdateLabelPrinterDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Name)
            .Must(name => name.Length != 0)
            .WithMessage("LabelPrinter name cannot be empty.");

        RuleFor(dto => dto.Type)
            .Must(type => type == 0 || type == 1)
            .WithMessage((_, country) => $"LabelPrinter type must be 0 or 1.");
    }
}
