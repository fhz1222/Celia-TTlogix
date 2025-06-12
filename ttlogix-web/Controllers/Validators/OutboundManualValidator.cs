using FluentValidation;
using TT.Services.Models;
using TT.Services.Models.ModelEnums;

namespace TT.Controllers.Validators
{
    public class OutboundManualValidator : AbstractValidator<OutboundManualDto>
    {
        public OutboundManualValidator()
        {
            RuleFor(dto => dto.CustomerCode)
                .NotEmpty().WithMessage("CustomerCodeCannotBeEmpty")
                .MaximumLength(10).WithMessage("CustomerCodeCannotExceedChars");

            RuleFor(dto => dto.WHSCode)
                .NotEmpty().WithMessage("WHSCodeCannotBeEmpty")
                .MaximumLength(7).WithMessage("WHSCodeCannotExceedChars");

            RuleFor(dto => dto.OSNo)
                .MaximumLength(15).WithMessage("OSNoCannotExceedChars");

            RuleFor(dto => dto.RefNo)
                .NotEmpty()
                .When(dto => dto.ManualType == ManualType.Empty)
                .WithMessage("RefNoCannotBeEmpty")
                .MaximumLength(30).WithMessage("RefNoCannotExceedChars");

            RuleFor(dto => dto.NewWHSCode)
                .NotEmpty()
                .When(dto => dto.ManualType == ManualType.WHSTransfer)
                .WithMessage("NewWHSCodeCannotBeEmpty");

            RuleFor(dto => dto.Remark)
                .MaximumLength(100).WithMessage("RemarksCannotExceedChars");
        }
    }
}
