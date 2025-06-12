using FluentValidation;
using TT.Core.Enums;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class InboundManualValidator : AbstractValidator<InboundManualDto>
    {
        public InboundManualValidator()
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
                .NotEmpty().WithMessage("RefNoCannotBeEmpty")
                .MaximumLength(30).WithMessage("RefNoCannotExceedChars");

            RuleFor(dto => dto.TransType)
                .NotEmpty().WithMessage("JobTypeCannotBeEmpty");

            RuleFor(dto => dto.TransType)
                .Must(dto => dto == InboundType.ManualEntry || dto == InboundType.Excess || dto == InboundType.Return)
                .When(dto => dto.TransType is { })
                .WithMessage("TransTypeMustBeManualExcessOrReturn");

            RuleFor(dto => dto.ETA)
                .NotEmpty().WithMessage("ETACannotBeEmpty");

            RuleFor(dto => dto.Remark)
                .MaximumLength(100).WithMessage("RemarksCannotExceedChars");
        }
    }
}
