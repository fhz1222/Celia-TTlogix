using FluentValidation;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class AddPkgQtyDtoValidator : AbstractValidator<AddPkgQtyDto>
    {
        public AddPkgQtyDtoValidator()
        {
            RuleFor(dto => dto.Qty)
                .GreaterThan(0).WithMessage("QtyMustBeGreaterThanZero");
        }
    }
}
