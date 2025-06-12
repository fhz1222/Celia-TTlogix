using FluentValidation;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class InboundDetailEntryModifyDtoValidator : AbstractValidator<InboundDetailEntryModifyDto>
    {
        public InboundDetailEntryModifyDtoValidator()
        {
            RuleFor(dto => dto.JobNo).NotEmpty().WithMessage("JobNoCannotBeEmpty");
            RuleFor(dto => dto.LineItem).GreaterThan(0).WithMessage("JobNoCannotBeEmpty");
            RuleFor(dto => dto.ProductCode).NotEmpty().WithMessage("ProductCodeCannotBeEmpty");

            RuleFor(dto => dto.QtyPerPkg).GreaterThan(0).WithMessage("QtyMustBeGreaterThanZero");
        }
    }
}
