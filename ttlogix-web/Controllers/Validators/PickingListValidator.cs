using FluentValidation;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class PickingListValidator : AbstractValidator<PickingListAllocateDto>
    {
        public PickingListValidator()
        {
            RuleFor(dto => dto.JobNo)
                .NotEmpty().WithMessage("JobNoCannotBeEmpty");

            RuleFor(dto => dto.LineItem)
                .GreaterThan(0).WithMessage("LineNoCannotBeZero");

            RuleFor(dto => dto.PID)
                .NotEmpty().WithMessage("PIDCannotBeEmpty");
        }
    }
}
