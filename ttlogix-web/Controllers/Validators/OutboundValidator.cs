using FluentValidation;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class OutboundValidator : AbstractValidator<OutboundDto>
    {
        public OutboundValidator()
        {
            RuleFor(dto => dto.CustomerCode)
                .NotEmpty().WithMessage("CustomerCodeCannotBeEmpty")
                .MaximumLength(10).WithMessage("CustomerCodeCannotExceedChars");

            RuleFor(dto => dto.Remark)
                .MaximumLength(100).WithMessage("RemarksCannotExceedChars");
            RuleFor(dto => dto.CommInvNo)
                .MaximumLength(1292).WithMessage("CommInvNoCannotExceedChars");
            RuleFor(dto => dto.DeliveryTo)
                .MaximumLength(20).WithMessage("DeliveryToCannotExceedChars");

            RuleFor(dto => dto.TransportNo)
                .MaximumLength(30).WithMessage("TransportNoCannotExceedChars");
        }
    }
}
