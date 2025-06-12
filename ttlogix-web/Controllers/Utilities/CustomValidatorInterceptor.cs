using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using TT.Services.Services.Utilities;

namespace TT.Controllers.Utilities
{
    public class CustomValidatorInterceptor : IValidatorInterceptor
    {
        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    error.ErrorMessage = new JsonResultError(error.ErrorMessage).ToJson();
                }
            }
            return result;
        }

        public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
            => commonContext;
    }
}
