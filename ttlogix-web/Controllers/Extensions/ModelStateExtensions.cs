using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TT.Controllers.Extensions
{
    public static class ModelStateExtensions
    {
        public static bool IsInvalid(this ModelStateDictionary msd) => !msd.IsValid;
    }
}
