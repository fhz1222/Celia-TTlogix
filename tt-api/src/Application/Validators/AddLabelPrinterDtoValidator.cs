using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.AddLabelPrinter;
using FluentValidation;
using System.Net;

namespace Application.Validators;

public class AddLabelPrinterDtoValidator : AbstractValidator<AddLabelPrinterDto>
{
    public AddLabelPrinterDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Ip)
            .Must(ip => ip.Length <= 15)
            .WithMessage("LabelPrinter IP is max 15 characters.");

        RuleFor(dto => dto.Ip)
            .Must(ip => ip.Length != 0)
            .WithMessage("LabelPrinter IP cannot be empty.");

        RuleFor(dto => dto.Ip)
            .Must(ip => ip.Count(x => x == '.') == 3 && IPAddress.TryParse(ip, out _))
            .WithMessage("LabelPrinter IP must be valid.");

        RuleFor(dto => dto.Name)
            .Must(name => name.Length != 0)
            .WithMessage("LabelPrinter name cannot be empty.");

        RuleFor(dto => dto.Type)
            .Must(type => type == 0 || type == 1)
            .WithMessage((_, country) => $"LabelPrinter type must be 0 or 1.");
    }
}
