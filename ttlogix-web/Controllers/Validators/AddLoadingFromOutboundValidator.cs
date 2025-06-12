using FluentValidation;
using System.Linq;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class AddLoadingFromOutboundValidator : AbstractValidator<AddLoadingFromOutboundDto>
    {
        public AddLoadingFromOutboundValidator()
        {
            RuleFor(dto => dto.OutJobNos)
                .Must(c => c != null && c.Any() && c.All(i => !string.IsNullOrEmpty(i)))
                .WithMessage("AtLeastOneOutboundJobNoMustBeSpecified");

            RuleFor(dto => dto.Loading).SetValidator(new LoadingAddValidator());
        }
    }
}
