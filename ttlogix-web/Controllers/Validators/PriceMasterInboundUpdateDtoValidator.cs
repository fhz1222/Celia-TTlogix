using FluentValidation;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class PriceMasterInboundUpdateDtoValidator : AbstractValidator<PriceMasterInboundUpdateDto>
    {
        public PriceMasterInboundUpdateDtoValidator()
        {
            RuleFor(dto => dto.Currency)
                .NotEmpty().WithMessage("InvalidCurrency");
        }
    }
}
