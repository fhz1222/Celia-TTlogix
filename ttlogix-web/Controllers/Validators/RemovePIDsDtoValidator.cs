using FluentValidation;
using System.Linq;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class RemovePIDsDtoValidator : AbstractValidator<RemovePIDsDto>
    {
        public RemovePIDsDtoValidator()
        {
            RuleFor(dto => dto.LineItem)
                .GreaterThan(0).WithMessage("IncorrectLineItem");

            RuleFor(dto => dto.PIDs)
                .Must(c => c != null && c.Any() && c.All(i => !string.IsNullOrEmpty(i)))
                .When(c => !c.RemoveAll)
                .WithMessage("AtLeastOnePIDMustBeSpecified");
        }
    }
}
