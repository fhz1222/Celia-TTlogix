using FluentValidation;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class OutboundDetailAddValidator : AbstractValidator<OutboundDetailAddDto>
    {
        public OutboundDetailAddValidator()
        {
            RuleFor(dto => dto.Qty)
                .GreaterThan(0).WithMessage("QtyToPickHaveToBeGraterThanZero");
            RuleFor(dto => dto.ProductCode)
                .NotNull().NotEmpty().WithMessage("MissingProductCode");
        }
    }
}
