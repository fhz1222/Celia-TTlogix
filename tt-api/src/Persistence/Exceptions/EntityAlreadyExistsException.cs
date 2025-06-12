using Application.Exceptions;

namespace Persistence.Repositories
{
    [Serializable]
    internal class EntityAlreadyExistsException : TtlogixApiException
    {
        public EntityAlreadyExistsException()
        {
        }

        public EntityAlreadyExistsException(string? message) : base(message)
        {
        }
    }
}