using FluentValidation;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class LoadingAddValidator : AbstractValidator<LoadingAddDto>
    {
        public LoadingAddValidator()
        {
            RuleFor(dto => dto.WHSCode)
               .NotEmpty().WithMessage("WHSCodeCannotBeEmpty")
               .MaximumLength(7).WithMessage("WHSCodeCannotExceedChars");

            RuleFor(dto => dto.CustomerCode)
                .NotEmpty().WithMessage("CustomerCodeCannotBeEmpty")
                .MaximumLength(10).WithMessage("CustomerCodeCannotExceedChars");

            RuleFor(dto => dto.RefNo)
               .NotEmpty().WithMessage("RefNoCannotBeEmpty")
               .MaximumLength(30).WithMessage("RefNoCannotExceedChars");

            RuleFor(dto => dto.Remark)
                .MaximumLength(100).WithMessage("RemarksCannotExceedChars.");
        }
    }
}
