using FluentValidation;
using System.Linq;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class UpdateBuyingPriceItemDtoValidator : AbstractValidator<UpdateBuyingPriceItemDto>
    {
        public UpdateBuyingPriceItemDtoValidator()
        {
            RuleFor(dto => dto.InJobNo)
                .NotEmpty().WithMessage("JobNoCannotBeEmpty");

            RuleFor(dto => dto.Currency)
                .NotEmpty().WithMessage("CurrencyCannotBeEmpty");

            RuleFor(dto => dto.Prices)
                .Must(c => c != null && c.Any() && c.All(i => i.LineItem > 0))
                .WithMessage("PleaseSelectLineToUpdate");
        }
    }
}
