using FluentValidation;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class PickEntryValidator : AbstractValidator<PickEntryDto>
    {
        public PickEntryValidator()
        {
            RuleFor(dto => dto.QtyToPick)
                .GreaterThan(0).WithMessage("PickingQtyMustBeGreaterThan0")
                .LessThanOrEqualTo(dto => dto.AvailableQty).WithMessage("PickingQtyMustBeLessEqualThanAvQty");

            RuleFor(dto => dto.SupplierID)
                .NotEmpty().WithMessage("SelectSupplierID");

            RuleFor(dto => dto.JobNo).NotEmpty();
            RuleFor(dto => dto.SupplierID).NotEmpty();
            RuleFor(dto => dto.ProductCode).NotEmpty();
        }
    }
}
