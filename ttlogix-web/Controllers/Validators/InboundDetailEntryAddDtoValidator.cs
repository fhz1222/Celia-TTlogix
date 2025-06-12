using FluentValidation;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class InboundDetailEntryAddDtoValidator : AbstractValidator<InboundDetailEntryAddDto>
    {
        public InboundDetailEntryAddDtoValidator()
        {
            RuleFor(dto => dto.JobNo)
                .NotEmpty().WithMessage("JobNoCannotBeEmpty")
                .MaximumLength(15).WithMessage("JobNoCannotExceedChars");

            RuleFor(dto => dto.ProductCode)
                .NotEmpty().WithMessage("ProductCodeCannotBeEmpty")
                .MaximumLength(30).WithMessage("ProductCodeCannotExceedChars");

            RuleFor(dto => dto.PackageType)
                .NotEmpty().WithMessage("PackageTypeCannotBeEmpty")
                .MaximumLength(7).WithMessage("PackageTypeCannotExceedChars");

            RuleFor(dto => dto.Qty)
                .GreaterThan(0).WithMessage("QtyMustBeGreaterThanZero");

            RuleFor(dto => dto.QtyPerPkg)
                .GreaterThan(0).WithMessage("QtyMustBeGreaterThanZero");

            RuleFor(dto => dto.ControlCode1)
                .MaximumLength(30).WithMessage("ControlCode1CannotExceedChars");
            RuleFor(dto => dto.ControlCode2)
                .MaximumLength(30).WithMessage("ControlCode2CannotExceedChars");
            RuleFor(dto => dto.ControlCode3)
                .MaximumLength(30).WithMessage("ControlCode3CannotExceedChars");
            RuleFor(dto => dto.ControlCode4)
                .MaximumLength(30).WithMessage("ControlCode4CannotExceedChars");
            RuleFor(dto => dto.ControlCode5)
                .MaximumLength(30).WithMessage("ControlCode5CannotExceedChars");
            RuleFor(dto => dto.ControlCode6)
                .MaximumLength(30).WithMessage("ControlCode6CannotExceedChars");
        }
    }
}
