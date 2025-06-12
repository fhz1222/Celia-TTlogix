using FluentValidation;
using System.Linq;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class SplitOutboundDtoValidator : AbstractValidator<SplitOutboundDto>
    {
        public SplitOutboundDtoValidator()
        {
            RuleFor(dto => dto.PickingListItemIds)
                .Must(c => c != null && c.Any() && c.All(i => !string.IsNullOrEmpty(i.JobNo) && i.LineItem > 0 && i.SeqNo > 0))
                .WithMessage("AtLeastOneOrderNoMustBeSpecified");
        }
    }
}
