using ServiceResult;

namespace TT.Services.Services.Utilities
{
    public static class Error<T>
    {
        public static InvalidResult<T> Get(string jsonErrorKey)
            => new(new JsonResultError(jsonErrorKey).ToJson());

        public static InvalidResult<T> Get(string jsonErrorKey, string paramKey, string paramValue) 
            => new(new JsonResultError(jsonErrorKey, paramKey, paramValue).ToJson());
    }
}
