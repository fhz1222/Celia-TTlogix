using FluentValidation;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class InboundValidator : AbstractValidator<InboundDto>
    {
        public InboundValidator()
        {
            RuleFor(dto => dto.CustomerCode)
                .NotEmpty().WithMessage("CustomerCodeCannotBeEmpty")
                .MaximumLength(10).WithMessage("CustomerCodeCannotExceedChars");

            RuleFor(dto => dto.SupplierID)
              .NotEmpty().WithMessage("SupplierIDCannotBeEmpty")
              .MaximumLength(10).WithMessage("SupplierIDCannotExceedChars");

            RuleFor(dto => dto.WHSCode)
                .NotEmpty().WithMessage("WHSCodeCannotBeEmpty")
                .MaximumLength(7).WithMessage("WHSCodeCannotExceedChars");

            RuleFor(dto => dto.RefNo)
                .MaximumLength(30).WithMessage("RefNoCannotExceedChars");

            RuleFor(dto => dto.Remark)
                .MaximumLength(100).WithMessage("RemarksCannotExceedChars");

            RuleFor(dto => dto.IM4No)
                .MaximumLength(18).WithMessage("IM4NoCannotExceedChars");
        }
    }
}
