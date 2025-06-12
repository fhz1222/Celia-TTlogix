using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.UseCases.Registration.Commands.UpdateAreaType;
using FluentValidation;

namespace Application.Validators;

public class UpdateAreaTypeDtoValidator : AbstractValidator<UpdateAreaTypeDto>
{
    public UpdateAreaTypeDtoValidator(IRepository repository)
    {
        RuleFor(dto => dto.Name)
            .Must(name => name.Length != 0)
            .WithMessage("AreaType Name cannot be empty.");
    }
}
