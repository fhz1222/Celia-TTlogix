using FluentValidation;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class StockTransferValidator : AbstractValidator<StockTransferDto>
    {
        public StockTransferValidator()
        {
            RuleFor(dto => dto.CustomerCode)
                .NotEmpty().WithMessage("CustomerCodeCannotBeEmpty")
                .MaximumLength(10).WithMessage("CustomerCodeCannotExceedChars");

            RuleFor(dto => dto.Remark)
                .MaximumLength(100).WithMessage("RemarksCannotExceedChars");

            RuleFor(dto => dto.CommInvNo)
                .MaximumLength(1292).WithMessage("CommInvNoCannotExceedChars");

            RuleFor(dto => dto.RefNo)
                .NotEmpty()
                .When(dto => dto.Status != Core.Enums.StockTransferStatus.Completed)
                .WithMessage("RefNoCannotBeEmpty")
                .MaximumLength(20).WithMessage("RefNoCannotExceed20Chars");
        }
    }
}
