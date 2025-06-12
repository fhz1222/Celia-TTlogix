using FluentValidation;
using System.Linq;
using TT.Services.Models;

namespace TT.Controllers.Validators
{
    public class JobNosValidator : AbstractValidator<JobNosDto>
    {
        public JobNosValidator()
        {
            RuleFor(dto => dto.JobNos)
                .Must(c => c != null && c.Any() && c.All(i => !string.IsNullOrEmpty(i)))
                .WithMessage("AtLeastOneJobNoMustBeSpecified");
        }
    }
}
