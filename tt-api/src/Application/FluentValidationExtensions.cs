using FluentValidation;

namespace Application;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> MustNot<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, bool> predicate)
    {
        return ruleBuilder.Must((x, val) => !predicate(val));
    }
}