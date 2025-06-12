using FluentValidation;
using System.Linq;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class DeleteLoadingDetailDtoValidator : AbstractValidator<DeleteLoadingDetailDto>
    {
        public DeleteLoadingDetailDtoValidator()
        {
            RuleFor(dto => dto.OrderNos)
                .Must(c => c != null && c.Any() && c.All(i => !string.IsNullOrEmpty(i)))
                .WithMessage("AtLeastOneOrderNoMustBeSpecified");
        }
    }
}
