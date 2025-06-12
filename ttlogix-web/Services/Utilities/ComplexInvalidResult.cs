using ServiceResult;
using System.Collections.Generic;
using System.Linq;

namespace TT.Services.Services.Utilities
{
    public class ComplexInvalidResult<T> : Result<T>
    {
        public ComplexInvalidResult(IEnumerable<string> errors) : base()
        {
            ComplexErrors = errors.ToList();
        }

        public List<string> ComplexErrors { get; private set; }

        public override List<string> Errors { get; }

        public override ResultType ResultType => ResultType.Invalid;

        public override T Data { get; }
    }
}
