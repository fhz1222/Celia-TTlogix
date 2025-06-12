using System.ComponentModel.DataAnnotations;

namespace TT.Services.Services.Utilities
{
    public class RequiredAsJsonErrorAttribute : RequiredAttribute
    {
        public override string FormatErrorMessage(string name) 
            => new JsonResultError("FieldCannotBeEmpty").ToJson();
    }
}
